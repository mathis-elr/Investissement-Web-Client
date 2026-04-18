using ApexCharts;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;
using Investissement_WebClient.Core.Modeles.ViewsModels;
using System.Text.Json;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestissementViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestissementService _investissementService;
        private readonly ITradeRepublicDataService _transactionService;

        /* PROPRIETES INVESTISSEMENT */
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal InvestissementTotal { get; set; }
        public IEnumerable<InvestissementParMois> InvestissementsParMois { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        //public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        /* Transactions */
        public IEnumerable<TransactionVM> Transactions { get; set; }

        public string Message { get; set; }

        public string Etat { get; set; }

        public string CodeSms { get; set; }

        /* Evolution actifs */
        public IEnumerable<InfoInvestParActif> InfoInvestParActif { get; set; }

        public InvestissementViewModel(IPatrimoineService patrimoineService, 
                                       IInvestissementService investissementService, 
                                       ITradeRepublicDataService transactionService)
        {
            _patrimoineService = patrimoineService;
            _investissementService = investissementService;
            _transactionService = transactionService;

            Transactions = [];
            InvestissementsParMois = [];

            InfoInvestParActif = [];

            Message = "Aucune demande de récupération de transactions en cours ...";
            Etat = "neutre";

            ErrorMessage = string.Empty;
            HasError = false;
        }
        
        private async Task LoadTransactions()
        {
            Transactions = await _investissementService.GetTransactions();
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _investissementService.CalculerInvestissementMoyenMensuel();
        }
        
        private async Task LoadInvestissementTotal(Dictionary<string, decimal> prixParActif)
        {
            InvestissementTotal = await _investissementService.CalculerValeurInvestissementTotal();
        }

        private async Task LoadInvestissementsParMois()
        {
            InvestissementsParMois = await _investissementService.GetInvestissementParMois(InvestissementMoyenMensuel);
        }

        public async Task LoadInfosInvestParActif(Dictionary<string, decimal> prixParActif)
        {
            InfoInvestParActif = await _investissementService.CalculerInfosInvestParActif(prixParActif);
        }

        private async Task<Dictionary<string, decimal>> LoadPrixParActif()
        {
            return await _investissementService.GetPrixParActif();
        }

        public async Task LoadData()
        {
            var prixParActif = await LoadPrixParActif();
            await LoadTransactions();
            await LoadInvestissementMoyenMensuel();
            await LoadInvestissementTotal(prixParActif);
            await LoadInvestissementsParMois();
            await LoadInfosInvestParActif(prixParActif);
        }

        public async Task LoadDataPrixParActif()
        {
            var prixParActif = await LoadPrixParActif();
            await LoadInfosInvestParActif(prixParActif);
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
            Etat = "verification";

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
            Etat = "recherche";
            Message = "Récupération des transactions, cette opération peut-être plus ou moins longue selon de la connexion ...";

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

        public string DeterminerSigne(decimal valeur)
        {
            return valeur switch
            {
                > 0 => "positive",
                < 0 => "negative",
                _ => "neutre"
            };
        }
    }
}
