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
        private readonly IInvestissementService _investissementService;
        private readonly ITrTransactionsService _transactionService;

        public InvestissementViewModel(IPatrimoineService patrimoineService, IInvestissementService investissementService, ITrTransactionsService transactionService)
        {
            _patrimoineService = patrimoineService;
            _investissementService = investissementService;
            _transactionService = transactionService;
        }


        /* PROPRIETES INVESTISSEMENT */
        public double InvestissementMoyenMensuel { get; set; } = 900;
        public decimal InvestissementTotal { get; set; } = 80000;
        public IEnumerable<InvestissementParMois> InvestissementsParMois { get; set; } = [];


        /* PROPRIETES REVENUS */
        public List<Revenu> Revenus { get; set; }  
        public decimal PartInvestissement { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        //public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }

        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        /* Transactions */
        public IEnumerable<TransactionVM> Transactions { get; set; } = [];

        public string status { get; set; } = "Aucune demande de récupération de transactions en cours ...";
        public string statusEtat { get; set; } = string.Empty;

        public string colorStatus { get; set; } = "goldenrod";

        private async Task LoadInvestissementsParMois()
        {
            //InvestissementsParMois = await _investirService.GetInvestissementParMois(InvestissementMoyenMensuel);
        }

        private async Task LoadTransactions()
        {
            Transactions = await _investissementService.GetTransactions();
        }

        public async Task LoadData()
        {
            await LoadInvestissementsParMois();
            await LoadTransactions();
        }

        public async Task RecupererTransactions()
        {
            status = "Tentative de connexion avec l'emetteur ...";

            string retour = await _transactionService.GetSms();

            if(retour == "sms_sent")
            {
                statusEtat = retour;
                colorStatus = "green";
                status = "Demande de sms effectué avec succès, verifier l'application trade republique";
            }
            else
            {
                statusEtat = "erreur";
                colorStatus = "red";
                status = "L'emetteur n'est pas disponible";
            }
        }
    }
}
