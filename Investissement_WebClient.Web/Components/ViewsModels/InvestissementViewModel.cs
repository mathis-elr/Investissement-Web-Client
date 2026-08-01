using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.API.PowensApi;
using Investissement_WebClient.Application.Services.API.TradeRepublicApi;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Web.GestionSession;
using System.Diagnostics;



namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class InvestissementViewModel(SessionService sessionService,
                                         IFluxInvestissementService fluxInvestissementService,
                                         ITradeRepublicApiService tradeRepublicApiService,
                                         IPowensApiService powensDataService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;
        private readonly ITradeRepublicApiService _tradeRepublicApiService = tradeRepublicApiService;
        private readonly IPowensApiService _powensDataService = powensDataService;


        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        // MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange.Invoke();

        // TRANSACTIONS
        public TradeRepublicAccesVM? TradeRepublicAcces { get; set; } = new TradeRepublicAccesVM();
        public bool IdentifiantsRequis => TradeRepublicAcces == null;
        public IEnumerable<FluxInvestissementDto> FluxInvestissement { get; set; } = [];
        public string Message { get; set; } = "Aucune demande en cours ...";
        public Etat Etat { get; set; } = Etat.Neutre;
        public string CodeSms { get; set; } = string.Empty;
        public bool DemandeEnCours { get; set; } = false;
        public bool VerificationEnCours { get; set; } = false;

        // INVESTISSEMENT HISTORIQUE
        public bool ChargementEncours { get; set; } = false;
        public decimal InvestissementMedianMensuel { get; set; }
        public decimal InvestissementTotal { get; set; }
        public IEnumerable<InvestissementParMoisVM> InvestissementsParMois { get; set; } = [];
        public IEnumerable<ValeurActifInfosDto> ValeurActifInfos { get; set; } = [];

        // EVOLUTION ACTIFS
        public IEnumerable<InfoParActifDto> InfoParActifPeriodeAll => ValeurActifInfos
            .Select(v => new InfoParActifDto 
            { 
                Actif = v.Actif,
                ValeurDetenue = v.ValeurInvestit,
                VariationValeur = v.VariationsParLapsTemps.Where(v => v.Key == LapsTemps.All).Select(v => v.Value.VariationValeur).FirstOrDefault(),
                VariationPourcentage = v.VariationsParLapsTemps.Where(v => v.Key == LapsTemps.All).Select(v => v.Value.VariationPourcentage).FirstOrDefault(),
            });

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
            ChargementEncours = true;

            try
            {
                await InitialiserSession();

                await LoadIdentifiantsRequis();
                if (IdentifiantsRequis)
                    Message = "Synchronisation de vos identifiants Trade Republic nécéssaire";
                await LoadFluxInvestissement();

                if (FluxInvestissement.Any())
                {
                    await LoadInvestissementMedianMensuel();

                    var prixParActif = await LoadPrixParActif();
                    await Task.WhenAll(
                        LoadInvestissementTotal(prixParActif),
                        LoadInvestissementsParMois(),
                        LoadValeurInfoParActif(prixParActif)
                    );
                }
            }
            finally
            {
                ChargementEncours = false;
            }
        }

        public async Task SaveAccesTR()
        {
            try
            {
                await _tradeRepublicApiService.SaveAcces(TradeRepublicAcces, IdUser);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        public async Task LoadValeurInfoParActif(Dictionary<string, decimal> prixParActif)
        {
            ValeurActifInfos = await _fluxInvestissementService.CalculerInfosInvestParActif(prixParActif, IdUser);
        }

        private async Task<Dictionary<string, decimal>> LoadPrixParActif()
        {
            return await _fluxInvestissementService.GetPrixParActif();
        }

        public async Task LoadDataPrixParActif()
        {
            var prixParActif = await LoadPrixParActif();
            await LoadValeurInfoParActif(prixParActif);
        }

        public async Task DemandeCodeSms()
        {
            DemandeEnCours = true;
            Message = "Tentative de connexion avec l'emetteur ...";

            try
            {
                (var codeStatut, var messageRecu) = await _tradeRepublicApiService.GetSms(IdUser);

                if(codeStatut != 200)
                {
                    ErrorMessage = messageRecu;
                    HasError = true;
                    return; 
                }

                Message = messageRecu;
                Etat = Etat.SmsRequis;

                NotifyStateChanged();
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
            VerificationEnCours = true;
            Message = "Vérification de la conformité du code ...";

            if (int.TryParse(CodeSms, out int codeSmsString) && CodeSms.Length!=4)
            {
                ErrorMessage = "Le code doit être composé de 4 chiffres.";
                HasError = true;
                return false;
            }

            try
            {
                var messageRecu = await _tradeRepublicApiService.ConfirmSms(CodeSms);
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
                if (await _tradeRepublicApiService.ChargerTransactions(IdUser))
                {
                    await LoadData();
                    return true;
                }
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
            Message = "Aucune demande en cours ...";
            DemandeEnCours = false;
            VerificationEnCours = false;

            NotifyStateChanged();   
        }

        private async Task InitialiserSession()
        {
            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;
        }

        private async Task LoadFluxInvestissement()
        {
            FluxInvestissement = await _fluxInvestissementService.GetFluxInvestissement(IdUser);
        }

        private async Task LoadInvestissementMedianMensuel()
        {
            InvestissementMedianMensuel = await _fluxInvestissementService.CalculerInvestissementMedianMensuel(IdUser);
        }

        private async Task LoadInvestissementTotal(Dictionary<string, decimal> prixParActif)
        {
            InvestissementTotal = await _fluxInvestissementService.CalculerValeurInvestissementTotal(IdUser);
        }

        private async Task LoadInvestissementsParMois()
        {
            InvestissementsParMois = await _fluxInvestissementService.GetInvestissementParMois(IdUser);
        }

        private async Task LoadIdentifiantsRequis()
        {
            TradeRepublicAcces = await _tradeRepublicApiService.GetTradeRepublicAcces(IdUser);
        }
    }
}
