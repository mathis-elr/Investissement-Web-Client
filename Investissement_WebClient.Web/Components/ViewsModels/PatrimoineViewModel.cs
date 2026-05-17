using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.ValeurPatrimoines;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Web.GestionSession;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class PatrimoineViewModel(SessionService sessionService,
                                     IValeurPatrimoineService valeurPatrimoineService, 
                                     IFluxInvestissementService fluxInvestissementService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IValeurPatrimoineService _valeurPatrimoineService = valeurPatrimoineService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;


        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        // DATAS INFOS PATRIMOINE
        public bool RecuparationEnCours { get; set; } = false;
        public decimal ValeurPatrimoineCourante { get; set; }
        private decimal ValeurInvestissementTotal { get; set; }
        public IEnumerable<VariationDto> Variations { get; set; } = [];

        // DATAS GRAPHIQUES
        public IEnumerable<BougieJournaliereCandleChartVM> BougiesJournalieresPlusOuMoinsValues { get; set; } = [];
        public IEnumerable<BougieJournaliereCandleChartVM> BougiesJournalieresValeurPatrimoineSurInvestissementTotal { get; set; } = [];
        public IEnumerable<ValeurTotaleParActifVM> ValeurParActifInvestit { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError {get; set;} = false;
        public string ErrorMessage {get; set;} = string.Empty;


        public async Task StartLoadData()
        {
            RecuparationEnCours = true;

            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;

            var prixParActif = await _fluxInvestissementService.GetPrixParActif();

            await LoadValeurPatrimoineCourante(prixParActif);

            if(ValeurPatrimoineCourante != 0)
            {
                await LoadValeurInvestissementTotale();
                await LoadVariationsPrix();

                // GRAPHIQUES
                await LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal();
                await LoadBougiesJournalieresPlusOuMoinsValues();
                await LoadProportionParActif(prixParActif);
            }

            RecuparationEnCours = false;
        }

        public string DeterminerClasse(decimal variationPrix)
        {
            return variationPrix switch
            {
                > 0 => "vert",
                < 0 => "rouge",
                _   => "gris"
            };
        }

        public string ToStringPourcentage(decimal valeur, string devise)
        {
            return valeur.ToString(devise, CultureInfo.GetCultureInfo("fr-FR"));
        }

        private async Task LoadValeurPatrimoineCourante(Dictionary<string, decimal> prixParActif)
        {
            try
            {
                ValeurPatrimoineCourante = await _fluxInvestissementService.CalculerValeurCourante(prixParActif, IdUser);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadValeurInvestissementTotale()
        {
            ValeurInvestissementTotal = await _fluxInvestissementService.CalculerValeurInvestissementTotal(IdUser);
        }

        private async Task LoadVariationsPrix()
        {
            if (ValeurPatrimoineCourante == 0) return;  
            Variations = await _valeurPatrimoineService.GetVariations(ValeurPatrimoineCourante, ValeurInvestissementTotal, IdUser);
        }

        private async Task LoadBougiesJournalieresPlusOuMoinsValues()
        {
            BougiesJournalieresPlusOuMoinsValues = await _valeurPatrimoineService.GetBougiesJournalieresPlusOuMoinsValues(IdUser);
        }

        private async Task LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal()
        {
            BougiesJournalieresValeurPatrimoineSurInvestissementTotal = await _valeurPatrimoineService.GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(IdUser);
        }

        private async Task LoadProportionParActif(Dictionary<string, decimal> prixParActif)
        {
            ValeurParActifInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }
    }
}
