using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.Investissement;
using Investissement_WebClient.Application.Services.Patrimoine;
using Investissement_WebClient.Application.ViewsModels.Graphiques;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class PatrimoineViewModel(IPatrimoineService patrimoineService, IInvestissementService investissementService)
    {
        private readonly IPatrimoineService _patrimoineService = patrimoineService;
        private readonly IInvestissementService _investissementService = investissementService;
        
        // DATAS INFOS PATRIMOINE
        public decimal ValeurPatrimoineCourante { get; set; }
        private decimal ValeurInvestissementTotal { get; set; }
        public IEnumerable<VariationDto> Variations { get; set; } = [];

        // DATAS GRAPHIQUES
        public IEnumerable<BougieJournaliereVM> BougiesJournalieresPlusOuMoinsValues { get; set; } = [];
        public IEnumerable<BougieJournaliereVM> BougiesJournalieresValeurPatrimoineSurInvestissementTotal { get; set; } = [];
        public IEnumerable<ProportionActifVM> ProportionParActif { get; set; } = [];
        public IEnumerable<ProportionTypeActifVM> ProportionParTypeActif { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError {get; set;} = false;
        public string ErrorMessage {get; set;} = string.Empty;


        public async Task LoadData()
        {
            var prixParActif = await _investissementService.GetPrixParActif();
            await LoadValeurPatrimoineCourante(prixParActif);
            await LoadValeurInvestissementTotale();
            await LoadVariationsPrix();
        }

        public async Task LoadDataGraphiques()
        {
            await LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal();
            await LoadBougiesJournalieresPlusOuMoinsValues();
            //await LoadProportionParActif();
            //await LoadProportionParTypeActif();
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
                ValeurPatrimoineCourante = await _investissementService.CalculerValeurCourante(prixParActif);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadValeurInvestissementTotale()
        {
            ValeurInvestissementTotal = await _investissementService.CalculerValeurInvestissementTotal();
        }

        private async Task LoadVariationsPrix()
        {
            if (ValeurPatrimoineCourante == 0) return;
            Variations = await _patrimoineService.GetVariations(ValeurPatrimoineCourante, ValeurInvestissementTotal);
        }

        private async Task LoadBougiesJournalieresPlusOuMoinsValues()
        {
            BougiesJournalieresPlusOuMoinsValues = await _patrimoineService.GetBougiesJournalieresPlusOuMoinsValues();
        }

        private async Task LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal()
        {
            BougiesJournalieresValeurPatrimoineSurInvestissementTotal = await _patrimoineService.GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal();
        }

        //private async Task LoadProportionParActif()
        //{
        //    ProportionParActif = await _patrimoineService.GetProportionParActifInvestit(ValeurPatrimoineCourante);
        //}

        //private async Task LoadProportionParTypeActif()
        //{
        //    ProportionParTypeActif = await _patrimoineService.GetProportionParTypeActifInvestit(ValeurPatrimoineCourante);
        //}
    }
}
