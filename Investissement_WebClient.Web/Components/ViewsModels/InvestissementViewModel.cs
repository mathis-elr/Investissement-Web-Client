using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.Services.TradeRepublicApi;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Web.GestionSession;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class InvestissementViewModel(SessionService sessionService,
                                         IFluxInvestissementService fluxInvestissementService,
                                         ITradeRepublicApiService transactionService,
                                         IPowensApiService powensDataService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;
        private readonly ITradeRepublicApiService _transactionService = transactionService;
        private readonly IPowensApiService _powensDataService = powensDataService;


        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        // MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange.Invoke();

        // TRANSACTIONS
        public IEnumerable<FluxInvestissementDto> FluxInvestissement { get; set; } = [];
        public string Message { get; set; } = "Aucune demande de récupération de transactions en cours ...";
        public Etat Etat { get; set; } = Etat.Neutre;
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
            await InitialiserSession();

            try
            {
                await _powensDataService.GetToken(codeRetour, IdUser);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        public async Task LoadData()
        {
            await InitialiserSession();

            await LoadFluxInvestissement();
            if (!FluxInvestissement.Any())
                return;

            await LoadInvestissementMoyenMensuel();

            var prixParActif = await LoadPrixParActif();
            await LoadInvestissementTotal(prixParActif);
            await LoadInvestissementsParMois();
            await LoadInfosInvestParActif(prixParActif);
        }

        public async Task LoadInfosInvestParActif(Dictionary<string, decimal> prixParActif)
        {
            InfoInvestParActif = await _fluxInvestissementService.CalculerInfosInvestParActif(prixParActif, IdUser);
        }

        private async Task<Dictionary<string, decimal>> LoadPrixParActif()
        {
            return await _fluxInvestissementService.GetPrixParActif();
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
                var messageRecu = await _transactionService.GetSms();
                Message = messageRecu;
                Etat = Etat.SmsRequis;

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

        public async Task<bool?> VerfierCodeSms()
        {
            Message = "Vérification de la conformité du code ...";

            if (int.TryParse(CodeSms, out int codeSmsString) && CodeSms.Length!=4)
            {
                ErrorMessage = "Le code doit être composé de 4 chiffres.";
                HasError = true;
                return false;
            }

            try
            {
                var messageRecu = await _transactionService.ConfirmSms(CodeSms);
                Message = messageRecu;

                return await ChargerTransactions();
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return null;
            }
        }

        public async Task<bool?> ChargerTransactions()
        {
            Message = "Récupération des transactions, cette opération peut être plus ou moins longue ...";

            NotifyStateChanged();

            try
            {
                if (await _transactionService.ChargerTransactions(IdUser))
                {
                    await LoadData();
                    return true;
                }
                FinDeDemande();
                return false;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
                return null;
            }
        }

        public void ReinitiliserGestionErreur()
        {
            ErrorMessage = string.Empty;
            HasError = false;
        }
        public void FinDeDemande()
        {
            Etat = Etat.Neutre;
            Message = "Aucune demande de récupération de transactions en cours ...";
            DemandeEnCours = false;

            NotifyStateChanged();   
        }

        private async Task InitialiserSession()
        {
            await _sessionService.Initialiser();

            IdUser = _sessionService.Id;
            PrenomUser = _sessionService.Prenom;
        }

        private async Task LoadFluxInvestissement()
        {
            FluxInvestissement = await _fluxInvestissementService.GetFluxInvestissement(IdUser);
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _fluxInvestissementService.CalculerInvestissementMoyenMensuel(IdUser);
        }

        private async Task LoadInvestissementTotal(Dictionary<string, decimal> prixParActif)
        {
            InvestissementTotal = await _fluxInvestissementService.CalculerValeurInvestissementTotal(IdUser);
        }

        private async Task LoadInvestissementsParMois()
        {
            InvestissementsParMois = await _fluxInvestissementService.GetInvestissementParMois(InvestissementMoyenMensuel, IdUser);
        }
    }
}
