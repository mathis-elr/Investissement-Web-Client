using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class PatrimoineViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestissementService _investissementService;
        
        public decimal ValeurPatrimoineCourante { get; set; }
        private decimal ValeurInvestissementTotal { get; set; }
        public VariationsDto Variations { get; set; } = new VariationsDto();

        public IEnumerable<BougieJournaliere> BougiesJournalieresPlusOuMoinsValues { get; set; } = [];
        public IEnumerable<BougieJournaliere> BougiesJournalieresValeurPatrimoineSurInvestissementTotal { get; set; } = [];

        public IEnumerable<ProportionActif> ProportionParActif { get; set; } = [];
        public IEnumerable<ProportionTypeActif> ProportionParTypeActif { get; set; } = [];

        public bool HasError {get; set;} = false;
        public string ErrorMessage {get; set;} = string.Empty;
        
        
        public PatrimoineViewModel(IPatrimoineService patrimoineService, IInvestissementService investissementService)
        {
            _patrimoineService = patrimoineService;
            _investissementService = investissementService;
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
            Variations = await _patrimoineService.GetVariations(ValeurPatrimoineCourante,  ValeurInvestissementTotal);
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

        public string DeterminerSigne(decimal variationPrix)
        {
            return variationPrix switch
            {
                > 0 => "positive",
                < 0 => "negative",
                _   => "neutre"
            };
        }
    }
}
