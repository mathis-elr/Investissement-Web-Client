using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;

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
        public VariationsDto Variations { get; set; } = new VariationsDto();
        public IEnumerable<ValeurPatrimoine> ValeursPatrimoine { get; set; } = [];
        
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

        private async Task LoadVariationsPrix()
        { 
            if (ValeurPatrimoineCourante == 0) return;
            Variations = await _patrimoineService.GetVariations(ValeurPatrimoineCourante);
        }

        public async Task LoadData()
        {
            await LoadValeurPatrimoineCourante();
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
