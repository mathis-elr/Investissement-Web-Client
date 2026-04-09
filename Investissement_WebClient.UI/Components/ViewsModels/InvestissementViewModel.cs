using ApexCharts;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestissementViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestirService _investirService;

        public InvestissementViewModel(IPatrimoineService patrimoineService, IInvestirService investirService)
        {
            _patrimoineService = patrimoineService;
            _investirService = investirService;
        }


        /* PROPRIETES INVESTISSEMENT */
        public double InvestissementMoyenMensuel { get; set; } = 900;
        public decimal InvestissementTotal { get; set; } = 80000;
        public IEnumerable<InvestissementParMois> InvestissementsParMois { get; set; } = [];


        /* PROPRIETES REVENUS */
        public List<Revenu> Revenus { get; set; }  
        public decimal PartInvestissement { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }

        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        /* Transactions */
        public IEnumerable<InvestissementGetDto> Investissements { get; set; } = [];


        private async Task LoadInvestissementsParMois()
        {
            InvestissementsParMois = await _investirService.GetInvestissementParMois(InvestissementMoyenMensuel);
        }

        private async Task LoadInvestissements()
        {
            Investissements = await _investirService.GetInvestissements();
        }

        public async Task LoadData()
        {
            await LoadInvestissementsParMois();
            await LoadInvestissements();
        }

        public async Task DeleteDernierInvest(InvestissementGetDto investissement)
        {
            await _investirService.DeleteDernierInvest(investissement);
            await _patrimoineService.DeleteHistoriquePatrimoinePeriode(investissement.DateInvest);
            await LoadInvestissements();
        }
    }
}
