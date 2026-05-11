using ApexCharts;
using Investissement_WebClient.Application.ApiResponse;
using Investissement_WebClient.Application.Services.CreditCoop;
using Investissement_WebClient.Domain;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Investissement_WebClient.Application.Services.Powens;

public class PowensDataService : IPowensDataService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly IFluxCreditCoopService _fluxCreditCoopService;

    private readonly HttpClient Client = new HttpClient
    {
        BaseAddress = new Uri("https://investissement-sandbox.biapi.pro/2.0/"),
        Timeout = TimeSpan.FromSeconds(10)
    };

    public PowensDataService(IDbContextFactory<InvestissementDbContext> dbFactory, IFluxCreditCoopService fluxCreditCoopService)
    {
        _dbFactory = dbFactory;
        _fluxCreditCoopService = fluxCreditCoopService;
    }

    public async Task<bool> ConnexionRequise()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var acces = await context.CreditCoopAcces.FirstOrDefaultAsync();
        return acces == null || acces.DateExpiration < DateTime.Now;
    }

    public async Task GetToken(string code)
    {
        if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));


        Console.WriteLine("Code : " + code);

        var accesDictionnary = new Dictionary<string, string>();
        accesDictionnary.Add("client_id", PowensAPIAcces.ClientId);
        accesDictionnary.Add("client_secret", PowensAPIAcces.ClientSecret);
        accesDictionnary.Add("code", code);
        using var bodyUrl = new FormUrlEncodedContent(accesDictionnary);

        var reponse = await Client.PostAsync("auth/token/access", bodyUrl);
        var codeStatus = (int)reponse.StatusCode;

        VerifierContenueReponse(reponse, codeStatus);

        var reponseString = await reponse.Content.ReadAsStringAsync();
        var reponseJson = JsonDocument.Parse(reponseString);
        var rootReponse = reponseJson.RootElement;

        switch (codeStatus)
        {
            case 200:
                if (rootReponse.TryGetProperty("access_token", out var accessToken))
                {
                    var token = accessToken.GetString() ?? string.Empty;
                    var idCompteCourant = await GetIdCompteCourant(token);
                    await SaveToken(token, idCompteCourant);
                }
                else
                {
                    throw new Exception("Réponse valide mais le token d'acces est inncessible");
                }
                break;
            
            case 400 or 401:
                if (rootReponse.TryGetProperty("code", out var codeErreur))
                {
                    var codeErreurString = codeErreur.GetString() ?? string.Empty;

                    switch (codeErreurString)
                    {
                        case "invalidClient":
                            throw new Exception("Les identifiants sont manquants ou erronés.");
                        case "invalidGrant":
                            throw new Exception("Le code n'est pas ou plus valide.");
                        default:
                            var hasDescrErreur =
                                rootReponse.TryGetProperty("description", out var descriptionErreur);
                            Console.WriteLine(hasDescrErreur
                                ? descriptionErreur.ToString()
                                : "Aucune infos supplémentaires.");
                            throw new Exception("Une erreur est survenue lors de la requete.");
                    }
                }
                break;
            
            default:
                throw new Exception("Erreur inconnue, code erreur:" + reponse.StatusCode);
        }
    }

    public async Task GetFlux(DateTime dateDebut, DateTime dateFin)
    {
        var creditCoopAcces = await GetToken();
        if(creditCoopAcces == null)
            throw new  Exception("Aucune instance du token est enregistré");
        
        var dateDebutString = dateDebut.ToString("yyyy-MM-dd");
        var dateFinString = dateFin.ToString("yyyy-MM-dd");
        var requete = $"users/me/accounts/{creditCoopAcces.IdCompteCourant}/transactions?min_date={dateDebutString}&max_date={dateFinString}&limit=500";

        var reponse = await RequeteGetAvecToken(creditCoopAcces.AccesToken, requete);
        
        var reponseString = await reponse.Content.ReadAsStringAsync();
        var transactions = JsonSerializer.Deserialize<PowensTransactionsApiResponse>(reponseString);

        await _fluxCreditCoopService.AddFluxCreditCoop(transactions?.Transactions);
    }

    private async Task SaveToken(string token, int idCompteCourant)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var anciens = await context.CreditCoopAcces.ToListAsync();
        context.CreditCoopAcces.RemoveRange(anciens);

        var newAcces = new CreditCoopAcces
        {
            AccesToken = token,
            IdCompteCourant = idCompteCourant,
            DateCreation = DateTime.Now,
            DateExpiration = DateTime.Now.AddDays(90)
        };

        await context.CreditCoopAcces.AddAsync(newAcces);
        await context.SaveChangesAsync();
    }

    private async Task<CreditCoopAcces?> GetToken()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return context.CreditCoopAcces.FirstOrDefault();
    }

    private async Task<HttpResponseMessage> RequeteGetAvecToken(string token, string requete)
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var reponse = await Client.GetAsync(requete);

        var codeStatus = (int)reponse.StatusCode;
        VerifierContenueReponse(reponse, codeStatus);

        return reponse;
    }

    private async Task<int> GetIdCompteCourant(string token)
    {
        var reponse = await RequeteGetAvecToken(token, "users/me/accounts");
        var reponseString = await reponse.Content.ReadAsStringAsync();
        var comptes = JsonSerializer.Deserialize<PowensComptesApiResponse>(reponseString);

        if (comptes?.Comptes == null || !comptes.Comptes.Any())
            throw new Exception("L'API n'a renvoyé aucun compte pour cet utilisateur.");

        var compteId = comptes.Comptes.FirstOrDefault(c => c.Type == "market")?.Id
                    ?? comptes.Comptes.FirstOrDefault(c => c.Type == "checking")?.Id
                    ?? comptes.Comptes.First().Id;

        return compteId;
    }

    private void VerifierContenueReponse(HttpResponseMessage reponse, int codeStatus)
    {
        if (reponse.Content.Headers.ContentType.MediaType == "application/json")
            return;

        switch (codeStatus)
        {
            case 403:
                throw new Exception("Erreur de pare feu.");
            case 404:
                throw new Exception("Erreur dans l'URL.");
            case 500:
                throw new Exception("Erreur d'accès au serveur.");
            default:
                throw new Exception("Erreur inconnue, code erreur:" + codeStatus);
        }
    }
}