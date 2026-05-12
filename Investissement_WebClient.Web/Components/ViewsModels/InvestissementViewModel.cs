using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.Services.TradeRepublicApi;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class InvestissementViewModel(IFluxInvestissementService fluxInvestissementService,
                                         ITradeRepublicApiService transactionService,
                                         IPowensApiService powensDataService)
    {
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;
        private readonly ITradeRepublicApiService _transactionService = transactionService;
        private readonly IPowensApiService _powensDataService = powensDataService;

        public event Action OnChange;
        public void NotifyStateChanged() => OnChange?.Invoke();

        // TRANSACTIONS
        public IEnumerable<FluxInvestissementDto> FluxInvestissement { get; set; } = [];
        public string Message { get; set; } = "Aucune demande de récupération de transactions en cours ...";
        public string Etat { get; set; } = "neutre";
        public string CodeSms { get; set; } = string.Empty;
        public bool DemandeEnCours { get; set; } = false;

        // INVESTISSEMENT MOYEN
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal InvestissementTotal { get; set; }
        public IEnumerable<InvestissementParMoisVM> InvestissementsParMois { get; set; } = [];

        // EVOLUTION ACTIFS
        public IEnumerable<InfoValeurParActifDto> InfoInvestParActif { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task FinaliserConnexionBanque(string codeRetour)
        {
            try
            {
                await _powensDataService.GetToken(codeRetour);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        public async Task LoadInfosInvestParActif(Dictionary<string, decimal> prixParActif)
        {
            InfoInvestParActif = await _fluxInvestissementService.CalculerInfosInvestParActif(prixParActif);
        }

        private async Task<Dictionary<string, decimal>> LoadPrixParActif()
        {
            return await _fluxInvestissementService.GetPrixParActif();
        }

        public async Task LoadData()
        {
            await LoadFluxInvestissement();
            if (!FluxInvestissement.Any())
                return;
            
            await LoadInvestissementMoyenMensuel();

            var prixParActif = await LoadPrixParActif();
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
            DemandeEnCours = true;
            Message = "Tentative de connexion avec l'emetteur ...";

            try
            {
                (string messageRecu, int CodeHtpp) = await _transactionService.GetSms();
                Etat = CodeHtpp < 299 && CodeHtpp >= 200 ? "sms-requis" : "error";
                Message = messageRecu;

                NotifyStateChanged();
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
            Message = "Vérification de la conformité du code ...";

            if (int.TryParse(CodeSms, out int codeSmsString) && CodeSms.Length!=4)
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

                await ChargerTransactions();
                NotifyStateChanged();
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
            Message = "Récupération des transactions, cette opération peut durer quelque minutes ...";

            try
            {
                int CodeHtpp = await _transactionService.ChargerTransactions();
                Etat = CodeHtpp < 299 && CodeHtpp >= 200 ? "succes" : "error";

                await LoadData();
                FinDeDemande();
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

        public void ReinitiliserGestionErreur()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
        public void FinDeDemande()
        {
            Etat = "neutre";
            Message = "Aucune demande de récupération de transactions en cours ...";
            DemandeEnCours = false;

            NotifyStateChanged();
        }

        private async Task LoadFluxInvestissement()
        {
            FluxInvestissement = await _fluxInvestissementService.GetFluxInvestissement();
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _fluxInvestissementService.CalculerInvestissementMoyenMensuel();
        }

        private async Task LoadInvestissementTotal(Dictionary<string, decimal> prixParActif)
        {
            InvestissementTotal = await _fluxInvestissementService.CalculerValeurInvestissementTotal();
        }

        private async Task LoadInvestissementsParMois()
        {
            InvestissementsParMois = await _fluxInvestissementService.GetInvestissementParMois(InvestissementMoyenMensuel);
        }
    }
}
