using ApexCharts;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;
using System.Text.Json;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestissementViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestissementService _investissementService;
        private readonly ITrTransactionsService _transactionService;

        /* PROPRIETES INVESTISSEMENT */
        public double InvestissementMoyenMensuel { get; set; }
        public decimal InvestissementTotal { get; set; }
        public IEnumerable<InvestissementParMois> InvestissementsParMois { get; set; }


        /* PROPRIETES REVENUS */
        public List<Revenu> Revenus { get; set; }
        public decimal PartInvestissement { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        //public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        /* Transactions */
        public IEnumerable<TransactionVM> Transactions { get; set; }

        public string Message { get; set; }

        public string Etat { get; set; }

        public string CodeSms { get; set; }

        public InvestissementViewModel(IPatrimoineService patrimoineService, 
                                       IInvestissementService investissementService, 
                                       ITrTransactionsService transactionService)
        {
            _patrimoineService = patrimoineService;
            _investissementService = investissementService;
            _transactionService = transactionService;

            Transactions = [];
            InvestissementsParMois = [];

            Message = "Aucune demande de récupération de transactions en cours ...";
            Etat = "neutre";

            ErrorMessage = string.Empty;
            HasError = false;
        }


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

        public async Task DemandeCodeSms()
        {
            Message = "Tentative de connexion avec l'emetteur ...";

            try
            {
                (string messageRecu, int CodeHtpp) = await _transactionService.GetSms();
                Etat = CodeHtpp < 299 && CodeHtpp >= 200 ? "sms-requis" : "error";
                Message = messageRecu;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
        }

        public async Task VerfierCodeSms()
        {
            Etat = "neutre";

            if(int.TryParse(CodeSms, out int codeSmsString) && CodeSms.Length!=4)
            {
                Etat = "sms-requis";
                ErrorMessage = "Le code doit être composé de 4 chiffres.";
                HasError = true;
                return;
            }

            try
            {
                (string messageRecu, int CodeHtpp) = await _transactionService.ConfirmSms(CodeSms);
                Etat = CodeHtpp < 299 && CodeHtpp >= 200 ? "succes" : "error";
                Message = messageRecu;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
        }

        public async Task ChargerTransactions()
        {
            Etat = "neutre";
            Message = "Récupération des transactions ...";

            try
            {
                int CodeHtpp = await _transactionService.ChargerTransactions();
                Etat = CodeHtpp < 299 && CodeHtpp >= 200 ? "succes" : "error";
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return;
            }
        }
    }
}
