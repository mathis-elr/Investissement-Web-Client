using Investissement_WebClient.Infrastructure.APIs.Powens.Responses;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Services.Encrypt;
using Investissement_WebClient.Application.Interfaces.APIs;
using Microsoft.Extensions.DependencyInjection;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Investissement_WebClient.Infrastructure.APIs.Powens
{
    public class PowensApiService : IPowensApiService
    {
        private readonly IBanqueAccesRepository _banqueAccesRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CryptOptions _optionsEncryption;
        private readonly ICryptService _encryptService;
        private readonly PowensApiOptions _options;
        private readonly HttpClient _httpClient;

        public PowensApiService(IBanqueAccesRepository banqueAccesRepository,
                                IOptions<CryptOptions> optionsEncryption,
                                IOptions<PowensApiOptions> options,
                                IServiceScopeFactory scopeFactory,
                                ICryptService encryptService,
                                HttpClient httpClient)
        {
            _banqueAccesRepository = banqueAccesRepository;
            _optionsEncryption = optionsEncryption.Value;
            _encryptService = encryptService;
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_options.BaseUri);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task GetToken(string code, int userId)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));

            var accesDictionnary = new Dictionary<string, string>();
            accesDictionnary.Add("client_id", _options.ClientId);
            accesDictionnary.Add("client_secret", _options.ClientSecret);
            accesDictionnary.Add("code", code);
            using var bodyUrl = new FormUrlEncodedContent(accesDictionnary);

            var reponse = await _httpClient.PostAsync(_options.TokenEndPoint, bodyUrl);
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
                        await SaveAcces(token, idCompteCourant, userId);
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

        public async Task GetFlux(DateTime dateDebut, DateTime dateFin, int userId)
        {
            var acces = await _banqueAccesRepository.GetByUserId(userId) ?? throw new Exception("Aucune instance du token est enregistré");
            var tokenClair = _encryptService.Decrypt(acces.AccesTokenCrypte, _optionsEncryption.MasterKey);

            var dateDebutString = dateDebut.ToString("yyyy-MM-dd");
            var dateFinString = dateFin.ToString("yyyy-MM-dd");
            var requete = $"{_options.AccountsEndPoint}/{acces.IdCompteCourant}/transactions?min_date={dateDebutString}&max_date={dateFinString}&limit=500";

            var reponse = await RequeteGetAvecToken(tokenClair, requete);

            var reponseString = await reponse.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<PowensTransactionsApiResponse>(reponseString);

            Console.WriteLine($"DEBUG_SYNC: 4. Données reçues de Powens, nombre de flux trouvés : {transactions?.Transactions?.Count ?? 0}");

            using var scope = _scopeFactory.CreateScope();
            var fluxBancaireService = scope.ServiceProvider.GetRequiredService<IFluxBancaireService>();

            var flux = transactions?.Transactions?
                .Select(t => new FluxBancaireImportDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Valeur = t.Valeur,
                    Libelle = t.Libelle ?? string.Empty
                })
                .ToList();

            await fluxBancaireService.AddFluxBancaire(flux, userId);
        }

        private async Task SaveAcces(string token, int idCompteCourant, int userId)
        {
            var acces = await _banqueAccesRepository.GetByUserId(userId);

            if (acces != null)
            {
                acces.AccesTokenCrypte = _encryptService.Encrypt(token, _optionsEncryption.MasterKey);
                acces.IdCompteCourant = idCompteCourant;
                acces.DateCreation = DateTime.Now;
                acces.DateExpiration = DateTime.Now.AddDays(90);
                await _banqueAccesRepository.Update(acces);
            }
            else
            {
                var newAcces = new BanqueAcces
                {
                    AccesTokenCrypte = _encryptService.Encrypt(token, _optionsEncryption.MasterKey),
                    IdCompteCourant = idCompteCourant,
                    DateCreation = DateTime.Now,
                    DateExpiration = DateTime.Now.AddDays(90),
                    UtilisateurId = userId
                };
                await _banqueAccesRepository.Add(newAcces);
            }
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
            var reponse = await RequeteGetAvecToken(token, _options.AccountsEndPoint);
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
}