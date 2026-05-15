using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.Services.FluxBancaires;
using Investissement_WebClient.Domain.Configurations;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Investissement_WebClient.Application.Services.PowensApi;

public class PowensApiService : IPowensApiService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly IFluxBancaireService _fluxBancaireService;
    private readonly HttpClient _httpClient;

    private readonly string _clientId = PowensApiConfiguration.ClientId;
    private readonly string _clientSecret = PowensApiConfiguration.ClientSecret;

    private readonly string _baseUri = PowensApiConfiguration.BaseUri;
    private readonly string _tokenEndPoint = PowensApiConfiguration.TokenEndPoint;
    private readonly string _accountsEndPoint = PowensApiConfiguration.AccountsEndPoint;

    public PowensApiService(IDbContextFactory<InvestissementDbContext> dbFactory, 
                            IFluxBancaireService fluxBancaireService,
                            HttpClient httpClient)
    {
        _dbFactory = dbFactory;
        _fluxBancaireService = fluxBancaireService;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseUri);
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task GetToken(string code)
    {
        if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));


        var accesDictionnary = new Dictionary<string, string>();
        accesDictionnary.Add("client_id", _clientId);
        accesDictionnary.Add("client_secret", _clientSecret);
        accesDictionnary.Add("code", code);
        using var bodyUrl = new FormUrlEncodedContent(accesDictionnary);

        var reponse = await _httpClient.PostAsync(_tokenEndPoint, bodyUrl);
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
        var tokenAcces = await GetToken() ?? throw new Exception("Aucune instance du token est enregistré");
        var dateDebutString = dateDebut.ToString("yyyy-MM-dd");
        var dateFinString = dateFin.ToString("yyyy-MM-dd");
        var requete = $"{_accountsEndPoint}/{tokenAcces.IdCompteCourant}/transactions?min_date={dateDebutString}&max_date={dateFinString}&limit=500";

        var reponse = await RequeteGetAvecToken(tokenAcces.AccesToken, requete);

        var reponseString = await reponse.Content.ReadAsStringAsync();
        var transactions = JsonSerializer.Deserialize<PowensTransactionsApiResponse>(reponseString);

        await _fluxBancaireService.AddFluxBancaire(transactions?.Transactions);
    }

    private async Task SaveToken(string token, int idCompteCourant)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var anciens = await context.BanqueAcces.ToListAsync();
        context.BanqueAcces.RemoveRange(anciens);

        var newAcces = new BanqueAcces
        {
            AccesToken = token,
            IdCompteCourant = idCompteCourant,
            DateCreation = DateTime.Now,
            DateExpiration = DateTime.Now.AddDays(90)
        };

        await context.BanqueAcces.AddAsync(newAcces);
        await context.SaveChangesAsync();
    }

    private async Task<BanqueAcces?> GetToken()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return context.BanqueAcces.FirstOrDefault();
    }

    private async Task<HttpResponseMessage> RequeteGetAvecToken(string token, string requete)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var reponse = await _httpClient.GetAsync(requete);

        var codeStatus = (int)reponse.StatusCode;
        VerifierContenueReponse(reponse, codeStatus);
        return reponse;
    }

    private async Task<int> GetIdCompteCourant(string token)
    {
        var reponse = await RequeteGetAvecToken(token, _accountsEndPoint);
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

        throw codeStatus switch
        {
            403 => new Exception("Erreur de pare feu."),
            404 => new Exception("Erreur dans l'URL."),
            500 => new Exception("Erreur d'accès au serveur."),
            _ => new Exception("Erreur inconnue, code erreur:" + codeStatus),
        };
    }
}