using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Infrastructure.APIs.Powens;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.Extensions.Options;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.DTO.Auth;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class SourcesViewModel(ITradeRepublicAccesRepository tradeRepublicAccesRepository,
                                  ICompteBanqueRepository compteBanqueRepository,
                                  IOptions<PowensApiOptions> options,
                                  IPowensApiService powensApiService,
                                  SessionService sessionService)
    {
        private readonly ITradeRepublicAccesRepository _tradeRepublicAccesRepository = tradeRepublicAccesRepository;
        private readonly ICompteBanqueRepository _compteBanqueRepository = compteBanqueRepository;
        private readonly IPowensApiService _powensApiService = powensApiService;
        private readonly PowensApiOptions _powensApiOptions = options.Value;
        private readonly SessionService _sessionService = sessionService;

        // CONNEXION BANQUE
        public string UrlConnexionPowens { get; set; } = string.Empty;
        public List<SourceDto> Sources { get; set; } = [];
        public bool AucuneSource => Sources.Count == 0;
        public SourceDto? SourceSelectionne { get; set; }


        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        //MAJ VUE
        public bool Chargement { get; set; } = false;
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        // GESTION D'ERREUR
        public string MessageErreur { get; set; } = string.Empty;
        public bool HasErreur { get; set; } = false;


        public async Task StartLoadData()
        {
            Chargement = true;

            try
            {
                await InitialiserSession();

                await Task.WhenAll(
                    LoadComptesBanque(),
                    LoadCompteTradeRepublic()
                );

                if (AucuneSource)
                    return;

                SourceSelectionne = Sources.First();
            }
            finally
            {
                Chargement = false;
            }
        }

        public async Task FinaliserAjoutBanque(int connectionBanqueId)
        {
            await InitialiserSession();

            try
            {
                await _powensApiService.SaveBanque(connectionBanqueId, IdUser);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }     

        public async Task InitialiserUrlConnexionPowens()
        {
            UrlConnexionPowens = await GetUrlConnexionPowens();
        }

        public async Task ChangerSourceSelectionne(SourceDto source)
        {
            SourceSelectionne = source;
        }

        private async Task InitialiserSession()
        {
            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;
        }

        private async Task LoadComptesBanque()
        {
            Sources = await _compteBanqueRepository.GetAllByUserId(IdUser);
        } 

        private async Task LoadCompteTradeRepublic()
        {
            var tradeRepublicAcces = await _tradeRepublicAccesRepository.GetByUserId(IdUser);
            if (tradeRepublicAcces != null)
                Sources.Add(tradeRepublicAcces);
        }

        private async Task<string> GetUrlConnexionPowens()
        {
            await _powensApiService.VerifierUtilisateurPowensExists(IdUser);

            var code = await _powensApiService.GenerateCodeTemporaireByUserId(IdUser);

            var fullConnectUrl = new Uri(new Uri(_powensApiOptions.BaseUri), _powensApiOptions.ConnectEndPoint);

            var encodedRedirect =
                Uri.EscapeDataString(_powensApiOptions.RedirectUri);

            return $"{fullConnectUrl}?client_id={_powensApiOptions.ClientId}&redirect_uri={encodedRedirect}&code={code}";
        }
    }
}