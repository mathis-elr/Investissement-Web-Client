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
            
            RecupererDonneesLineQuantiteInvestitParDate();
            RecupererDonneesLineValeurPatrimoineParDate();
        }
        
        public async Task LoadValeurPatrimoineCourant()
        {
            Console.WriteLine("recuperation de la valeur du patrimoine actuel ...");
            valeurPatrimoineCourante = await patrimoine.GetValeurPatrimoineCourant();
            RecupererDonneesPieProportionParActif();
        }
        public void LoadVariationPrix()
        {
            Console.WriteLine("recuperation de la variation de prix ...");
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
