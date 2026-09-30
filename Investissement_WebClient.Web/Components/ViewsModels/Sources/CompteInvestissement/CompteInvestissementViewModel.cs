using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Application.DTO;

namespace Investissement_WebClient.Web.Components.ViewsModels.Sources.CompteInvestissement
{
    public class CompteInvestissementViewModel(IPositionInvestissementService positionInvestissementService,
                                               IPowensApiService powensApiService,
                                               SessionService sessionService)
    {
        private readonly IPositionInvestissementService _positionInvestissementService = positionInvestissementService;
        private readonly IPowensApiService _powensApiService = powensApiService;
        private readonly SessionService _sessionService = sessionService;

        public SourceDto CompteInvestissementCourant { get; set; } = null!;

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        //MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public bool ActionEnCours { get; set; } = false;

        public string UrlReconnectionPowens { get; set; } = string.Empty;

        // DATAS
        public IEnumerable<PositionInvestissementDto> PositionsInvestissement { get; set; } = [];
        public decimal ValeureTotale => PositionsInvestissement.Sum(p => p.Quantite * p.PrixCourant);

        public async Task StartLoadData(SourceDto sourceSelectionnee)
        {
            ActionEnCours = true;

            try
            {
                await InitialiserSession();

                CompteInvestissementCourant = sourceSelectionnee;

                await LoadPositions();
                await SetUrlReconnection();
            }
            finally
            {
                ActionEnCours = false;
            }
        }

        private async Task SetUrlReconnection()
        {
            UrlReconnectionPowens = await _powensApiService.GetUrlReconnexionPowens(IdUser, CompteInvestissementCourant.Id);
        }

        private async Task InitialiserSession()
        {
            await _sessionService.VerifierInitialisation();
            IdUser = _sessionService.Id;
        }

        private async Task LoadPositions()
        {
            PositionsInvestissement = await _positionInvestissementService.GetPositionsByCompte(CompteInvestissementCourant.Id);
        }
    }
}
