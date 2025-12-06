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

        public List<ChartsLinesPrix> ListPointsLineQuantiteInvesitParDate { get; set; }
        public List<ChartsLinesPrix> ListPointsLineValeurPatrimoineParDate { get; set; }
        public List<ChartPieProportionParActif> ListProportionParActif { get; set; }


        public async Task LoadValeurPatrimoineCourant()
        {
            await patrimoine.CalculerQuantiteEurParActif();
            patrimoine.CalculerValeurPatrimoineCourant();
            valeurPatrimoineCourante = patrimoine.GetValeurPatrimoineCourant();
        }

        public void LoadVariationPrix()
        {
            patrimoine.CalculerVariationPrix();
            variationPrix = patrimoine.GetVariationPrix();
        }

        public void LoadQuantiteParDate()
        {
            ListPointsLineQuantiteInvesitParDate = patrimoine.GetQuantiteInvestitParDate();
        }

        public void LoadValeurPatrimoineParDate()
        {
            ListPointsLineValeurPatrimoineParDate = patrimoine.GetValeurPatrimoineParDate();
        }

        public void LoadProportionParActif()
        {
            ListProportionParActif = patrimoine.GetProportionParActif();
        }

        public PatrimoineViewModel()
        {
            IPatrimoineSQLite iPatrimoine = new PatrimoineSQLite(BDDService.ConnectionString);
            IMarketDataService iMarketDataService = new YahooDataService();
            patrimoine = new Patrimoine(iPatrimoine, iMarketDataService);

            valeurPatrimoineCourante = "-- ----,--";
            variationPrix = "00,00";

            patrimoine.LoadQuantiteInvestitParActif(); //on l'appelle dans le constructeur car on ne veux aps qu'il se re-calcule a chaque fois que le prix change
            LoadQuantiteParDate();
            LoadValeurPatrimoineParDate();
        }
    }
}
