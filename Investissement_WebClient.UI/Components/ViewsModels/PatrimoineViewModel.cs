using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Repository.SQLite;
using Investissement_WebClient.Data.Repository.API;
using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Core;
using System.Diagnostics;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class PatrimoineViewModel
    {
        private readonly Patrimoine patrimoine;
        public string valeurPatrimoineCourante { get; set; }
        public string variationPrix { get; set; }


        public async Task LoadValeurPatrimoineCourant()
        {
            double valeurPatrimoineCouranteDouble = await patrimoine.GetValeurPatrimoineCourant();
            valeurPatrimoineCourante = valeurPatrimoineCouranteDouble.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
        }
        public void LoadVariationPrix()
        {

        }


        public PatrimoineViewModel()
        {
            IPatrimoineSQLite iPatrimoine = new PatrimoineSQLite(BDDService.ConnectionString);
            IMarketDataService iMarketDataService = new YahooDataService();
            patrimoine = new Patrimoine(iPatrimoine, iMarketDataService);

            valeurPatrimoineCourante = "-- ----,--";
            variationPrix = "--,--";

            LoadValeurPatrimoineCourant();
            LoadVariationPrix();
        }

    }
}
