using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Web.GestionSession;

namespace Investissement_WebClient.Web.Components.ViewsModels.Sources
{
    public class SourcesViewModel(ICompteTradeRepubliqueService tradeRepubliqueService,
                                  ICompteBanqueService compteBanqueService,
                                  IPowensApiService powensApiService,
                                  SessionService sessionService)
    {
        private readonly ICompteTradeRepubliqueService _tradeRepubliqueService = tradeRepubliqueService;
        private readonly ICompteBanqueService _compteBanqueService = compteBanqueService;
        private readonly IPowensApiService _powensApiService = powensApiService;
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

                var trAccesTask = LoadCompteTradeRepublic();

                await Task.WhenAll(
                    LoadComptesBanque(),
                    trAccesTask
                );

                var trAcces = await trAccesTask;

                if (trAcces != null)
                    Sources.Add(trAcces);

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

        public async Task ChangerSourceSelectionne(SourceDto source)
        {
            SourceSelectionne = source;
        }

        private async Task InitialiserSession()
        {
            await _sessionService.VerifierInitialisation();
            IdUser = _sessionService.Id;
        }

        private async Task LoadComptesBanque()
        {
            Sources = await _compteBanqueService.GetAllByUserId(IdUser);
        } 

        private async Task<SourceDto?> LoadCompteTradeRepublic()
        {
            return await _tradeRepubliqueService.GetByUserId(IdUser);
        }

        public async Task InitialiserUrlConnexionPowens()
        {
            try
            {
                UrlConnexionPowens = await _powensApiService.GetUrlConnexionPowens(IdUser);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }
    }
}