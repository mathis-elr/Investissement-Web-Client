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
        
        
        private async Task LoadValeurPatrimoineCourante()
        {
            ValeurPatrimoineCourante = await _patrimoineService.CalculerValeurPatrimoineCourante();
        }

        private async Task LoadVariationsPrix()
        {
           Variations = await _patrimoineService.GetVariations(ValeurPatrimoineCourante);
        }

        public async Task LoadData()
        {
            await LoadValeurPatrimoineCourante();
            await LoadVariationsPrix();
        }

        public string DeterminerSigne(double variationPrix)
        {
            var v = Math.Round(variationPrix, 2);
    
            return v switch
            {
                > 0 => "positive",
                < 0 => "negative",
                _   => "neutre"
            };
        }
    }
}
