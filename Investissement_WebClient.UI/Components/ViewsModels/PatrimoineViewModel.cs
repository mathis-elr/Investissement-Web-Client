using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Repository.SQLite;
using Investissement_WebClient.Data.Repository.Services;
using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Core;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class PatrimoineViewModel
    {
        private readonly Patrimoine patrimoine;
        public string valeurPatrimoineCourante { get; set; } = "-- ----,--";
        public string variationPrix { get; set; } = "00,00";

        public List<ChartsLinesPrix> ListPointsLineQuantiteInvesitParDate { get; set; }
        public List<ChartsLinesPrix> ListPointsLineValeurPatrimoineParDate { get; set; }
        public List<ChartPieProportionParActif> ListProportionParActif { get; set; }

        public PatrimoineViewModel()
        {
            IPatrimoineSQLite iPatrimoine = new PatrimoineSQLite(BDDService.ConnectionString);
            IMarketDataService iMarketDataService = new YahooDataService();
            patrimoine = new Patrimoine(iPatrimoine, iMarketDataService);
        }
        
        public async Task LoadValeurPatrimoineCourant()
        {
            valeurPatrimoineCourante = await patrimoine.GetValeurPatrimoineCourant();
        }
        public void LoadVariationPrix()
        {
            variationPrix = patrimoine.GetVariationPrix();
        }
        
        public void RecupererDonneesLineQuantiteInvestitParDate()
        {
            ListPointsLineQuantiteInvesitParDate = patrimoine.GetQuantiteInvestitParDate();
        }
        public void RecupererDonneesLineValeurPatrimoineParDate()
        {
            ListPointsLineValeurPatrimoineParDate = patrimoine.GetValeurPatrimoineParDate();
        }
        public void RecupererDonneesPieProportionParActif()
        {
            ListProportionParActif = patrimoine.GetProportionParActif();
        }
    }
}
