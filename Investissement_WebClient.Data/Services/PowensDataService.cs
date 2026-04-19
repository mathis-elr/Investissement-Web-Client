using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

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

    public async Task GetToken(string code)
    {
        if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));

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
                    await SaveToken(token);
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
        
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", creditCoopAcces.AccesToken);
        var dateDebutString = dateDebut.ToString("yyyy-MM-dd");
        var dateFinString = dateFin.ToString("yyyy-MM-dd");
        var reponse = await Client.GetAsync($"users/me/transactions?min_date={dateDebutString}&max_date={dateFinString}&limit=500");
        
        var codeStatus = (int)reponse.StatusCode;
        VerifierContenueReponse(reponse, codeStatus);
        
        var reponseString = await reponse.Content.ReadAsStringAsync();
        var transactions = JsonSerializer.Deserialize<PowensTransactionsResponse>(reponseString);

        await _fluxCreditCoopService.AddFluxCreditCoop(transactions?.Transactions);
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

    private async Task SaveToken(string token)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var creditCoopAcces = await GetToken();
        if(creditCoopAcces == null)
            throw new  Exception("Aucune instance du token est enregistré");

        if (creditCoopAcces.AccesToken == null)
        {
            var newAcces = new CreditCoopAcces
            {
                AccesToken = token,
                DateCreation = DateTime.Now,
            };
            await context.CreditCoopAcces.AddAsync(newAcces);
        }
        else
        {
            creditCoopAcces.AccesToken = token;
            creditCoopAcces.DateCreation = DateTime.Now;
        }

        await context.SaveChangesAsync();
    }

    private async Task<CreditCoopAcces?> GetToken()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return context.CreditCoopAcces.FirstOrDefault();
    }
}