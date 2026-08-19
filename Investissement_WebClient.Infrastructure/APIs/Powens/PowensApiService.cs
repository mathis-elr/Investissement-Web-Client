using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Services.Encrypt;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure.APIs.Powens.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Investissement_WebClient.Infrastructure.APIs.Powens
{
    public class PowensApiService : IPowensApiService
    {
        private readonly IUtilisateurPowensRepository _utilisateurPowensRepository;
        private readonly ICompteBanqueRepository _compteBanqueRepository;
        private readonly IBanqueRepository _banqueAccesRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CryptOptions _optionsEncryption;
        private readonly ICryptService _encryptService;
        private readonly PowensApiOptions _options;
        private readonly HttpClient _httpClient;

        public PowensApiService(IUtilisateurPowensRepository utilisateurPowensRepository,
                                ICompteBanqueRepository compteBanqueRepository,
                                IBanqueRepository banqueAccesRepository,
                                IOptions<CryptOptions> optionsEncryption,
                                IOptions<PowensApiOptions> options,
                                IServiceScopeFactory scopeFactory,
                                ICryptService encryptService,
                                HttpClient httpClient)
        {
            _utilisateurPowensRepository = utilisateurPowensRepository;
            _compteBanqueRepository = compteBanqueRepository;
            _banqueAccesRepository = banqueAccesRepository;
            _optionsEncryption = optionsEncryption.Value;
            _encryptService = encryptService;
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_options.BaseUri);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }


        public async Task CreeNouvelUtilisateur(int userId)
        {
            var utilisateurExistant = await _utilisateurPowensRepository.GetByUserId(userId);

            if (utilisateurExistant != null)
                return;

            var accesDictionnary = new Dictionary<string, string>();
            accesDictionnary.Add("client_id", _options.ClientId);
            accesDictionnary.Add("client_secret", _options.ClientSecret);
            using var bodyUrl = new FormUrlEncodedContent(accesDictionnary);

            var reponse = await _httpClient.PostAsync(_options.NouvelUtilisateurEndPoint, bodyUrl);
            var codeStatus = (int)reponse.StatusCode;

            VerifierContenueReponse(reponse, codeStatus);

            var reponseString = await reponse.Content.ReadAsStringAsync();
            var reponseJson = JsonDocument.Parse(reponseString);
            var rootReponse = reponseJson.RootElement;

            var newUser = JsonSerializer.Deserialize<PowensNouvelUtilisateurApiResponse>(reponseString);
            if (newUser == null || newUser.AuthToken == null) throw new Exception("Echec de la création d'un nouvel utilisateur powens.");

            await SaveUtiliteurPowens(userId, newUser.AuthToken, newUser.IdUser);
        }

        public async Task VerifierUtilisateurPowensExists(int userId)
        {
            var utilisateurPowens = await _utilisateurPowensRepository.GetByUserId(userId);
            if (utilisateurPowens != null)
                return;

            await CreeNouvelUtilisateur(userId);
        }

        public async Task<string> GenerateCodeTemporaireByUserId(int userId)
        {
            var utilisateurPowens = await _utilisateurPowensRepository.GetByUserId(userId) ?? throw new Exception("Aucun utilisateur powens est associé à ce compte.");
            var tokenClair = _encryptService.Decrypt(utilisateurPowens.AccessTokenCrypte, _optionsEncryption.MasterKey);

            var reponse = await RequeteGetAvecToken(tokenClair, _options.CodeByTokenEndPoint);
            var reponseString = await reponse.Content.ReadAsStringAsync();
            var codeTemporaire = JsonSerializer.Deserialize<PowensCodeTemporaireApiResponse>(reponseString);

            return codeTemporaire?.Code ?? throw new Exception("Echec de la récupération d'un code temporaire via le token");
        }

        public async Task SaveBanque(int connectionBanqueId, int userId)
        {
            var utilisateurPowens = await _utilisateurPowensRepository.GetByUserId(userId) ?? throw new Exception("Aucun utilisateur powens est associé à ce compte.");
            var tokenClair = _encryptService.Decrypt(utilisateurPowens!.AccessTokenCrypte, _optionsEncryption.MasterKey);

            var idConnector = await GetIdConnector(tokenClair, connectionBanqueId);
            var newBanque = new Banque
            {
                IdConnectionPowens = connectionBanqueId,
                IdConnectorPowens = idConnector,
                Nom = await GetNomBanque(tokenClair, idConnector),
                DateCreation = DateTime.Now,
                DateExpiration = DateTime.Now.AddDays(90),
                UtilisateurPowensId = utilisateurPowens.Id
            };
            await _banqueAccesRepository.Add(newBanque);

            await SaveComptes(tokenClair, newBanque.Id, connectionBanqueId);
        }

        public async Task GetFlux(DateTime dateDebut, DateTime dateFin, CompteBanque compteBanque)
        {
            var utilisateurPowens = compteBanque.Banque.UtilisateurPowens;
            var tokenClair = _encryptService.Decrypt(utilisateurPowens.AccessTokenCrypte, _optionsEncryption.MasterKey);

            var dateDebutString = dateDebut.ToString("yyyy-MM-dd");
            var dateFinString = dateFin.ToString("yyyy-MM-dd");
            var requete = $"{_options.AccountsEndPoint}/{compteBanque.IdComptePowens}/transactions?min_date={dateDebutString}&max_date={dateFinString}&limit=500";

            var reponse = await RequeteGetAvecToken(tokenClair, requete);

            var reponseString = await reponse.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<PowensTransactionsApiResponse>(reponseString);

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

            await fluxBancaireService.AddFluxBancaire(flux, utilisateurPowens.UtilisateurId, compteBanque.Id);
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

        private async Task<string> GetNomBanque(string token, int idConnector)
        {
            var reponse = await RequeteGetAvecToken(token, string.Format(_options.ConnectorEndPoint, idConnector));
            var reponseString = await reponse.Content.ReadAsStringAsync();
            var comptes = JsonSerializer.Deserialize<PowensConnectorApiResponse>(reponseString);

            if (comptes?.NomBanque == null)
                throw new Exception("L'API n'a renvoyé aucun connector pour cette banque.");

            return comptes.NomBanque;
        }

        private async Task<int> GetIdConnector(string token, int connectionBanqueId)
        {
            var reponse = await RequeteGetAvecToken(token, string.Format(_options.ConnectionsEndPoint, connectionBanqueId));
            var reponseString = await reponse.Content.ReadAsStringAsync();
            var compte = JsonSerializer.Deserialize<PowensConnectionApiResponse>(reponseString);

            return compte == null ? throw new Exception("L'API n'a renvoyé aucune connection pour cet utilisateur.") : compte.IdConnector;
        }

        private async Task SaveUtiliteurPowens(int userId, string token, int idUtilisateurPowens)
        {
            var nouvelUtilisateur = new UtilisateurPowens
            {
                IdUtilisateurPowens = idUtilisateurPowens,
                AccessTokenCrypte = _encryptService.Encrypt(token, _optionsEncryption.MasterKey),
                UtilisateurId = userId,
            };
            await _utilisateurPowensRepository.Add(nouvelUtilisateur);
        }

        private async Task SaveComptes(string token, int idBanqueLocal, int connectionBanqueId)
        {
            var reponse = await RequeteGetAvecToken(token, string.Format(_options.AccountsConnectionEndPoint, connectionBanqueId));
            var reponseString = await reponse.Content.ReadAsStringAsync();
            var comptes = JsonSerializer.Deserialize<PowensComptesApiResponse>(reponseString);

            if (comptes?.Comptes == null || !comptes.Comptes.Any())
                throw new Exception("L'API n'a renvoyé aucun compte pour cet utilisateur.");

            foreach(var compte in comptes.Comptes)
            {
                var newCompte = new CompteBanque
                {
                    IdComptePowens = compte.Id,
                    Nom = compte?.NomCompte ?? "Inconnue",
                    TypePowens = compte?.Type ?? "Inconnu",
                    BanqueId = idBanqueLocal
                };

                await _compteBanqueRepository.Add(newCompte);
            }
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