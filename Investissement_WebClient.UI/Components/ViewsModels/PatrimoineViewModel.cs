using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class PatrimoineViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        
        public PatrimoineViewModel(IPatrimoineService patrimoineService)
        {
            _patrimoineService = patrimoineService;
        }
        
        public double ValeurPatrimoineCourante { get; set; }
        private double ValeurInvestissementTotal { get; set; }
        public VariationsDto Variations { get; set; } = new VariationsDto();
        public IEnumerable<BougieJournaliere> BougiesJournalieres { get; set; } = [];

        public IEnumerable<ProportionActif> ProportionParActif { get; set; } = [];
        public IEnumerable<ProportionTypeActif> ProportionParTypeActif { get; set; } = [];

        public bool HasError {get; set;} = false;
        public string ErrorMessage {get; set;} = string.Empty;
        
        
        private async Task LoadValeurPatrimoineCourante()
        {
            try
            {
                ValeurPatrimoineCourante = await _patrimoineService.CalculerValeurPatrimoineCourante();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadValeurInvestissementTotale()
        {
            ValeurInvestissementTotal = await _patrimoineService.CalculerValeurInvestissementTotal();
        }

        private async Task LoadVariationsPrix()
        { 
            if (ValeurPatrimoineCourante == 0) return;
            Variations = await _patrimoineService.GetVariations(ValeurPatrimoineCourante,  ValeurInvestissementTotal);
        }

        public async Task LoadBougieJournalieres()
        {
            BougiesJournalieres = await _patrimoineService.GetBougiesJournalieres();
        }

        private async Task LoadProportionParActif()
        {
            ProportionParActif = await _patrimoineService.GetProportionParActifInvestit(ValeurPatrimoineCourante);
        }

        private async Task LoadProportionParTypeActif()
        {
            ProportionParTypeActif = await _patrimoineService.GetProportionParTypeActifInvestit(ValeurPatrimoineCourante);
        }

        public async Task LoadDataPies()
        {
            await LoadProportionParActif();
            await LoadProportionParTypeActif();
        }

        public async Task LoadData()
        {
            await LoadValeurPatrimoineCourante();
            await LoadValeurInvestissementTotale();
            await LoadVariationsPrix();
        }

        public string DeterminerSigne(double variationPrix)
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
