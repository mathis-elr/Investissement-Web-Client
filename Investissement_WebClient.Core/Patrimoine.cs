using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using System.Diagnostics;

namespace Investissement_WebClient.Core
{
    public class Patrimoine
    {
        private readonly IPatrimoineSQLite _IPatrimoine;
        private readonly IMarketDataService _IMarketDataService;

        private double valeurTotalePatrimoine { get; set; }
        private double quantiteTotaleInvestit { get; set; }
        private double variationPrix { get; set; }
        private Dictionary<string, (double, double)> dictionnaireQuantiteParActif { get; set; }
        private Dictionary<string, double> dictionnairePrixParActif { get; set; }

        public Patrimoine(IPatrimoineSQLite iPatrimoine, IMarketDataService iMarketDataService)
        {
            _IPatrimoine = iPatrimoine;
            _IMarketDataService = iMarketDataService;
        }

        public void CalculerQuantiteTotaleInvestit()
        {
            quantiteTotaleInvestit = dictionnaireQuantiteParActif.Sum(actif => actif.Value.Item2);
        }

        public void LoadQuantiteInvestitParActif()
        {
            dictionnaireQuantiteParActif = _IPatrimoine.GetQuantiteInvestitParActif();
        }

        public async Task LoadPrixParActif()
        {
            dictionnairePrixParActif = await _IMarketDataService.GetPrixActuelAsync(_IPatrimoine.GetSymboles());
        }

        public async Task CalculerValeurPatrimoineCourant()
        {
            await this.LoadPrixParActif();
            valeurTotalePatrimoine = 0;

            foreach (KeyValuePair<string, string> symboleParActif in _IPatrimoine.GetSymboleParActif())
            {
                double quantite = dictionnaireQuantiteParActif[symboleParActif.Key].Item1;

                double prix = 0;
                if (dictionnairePrixParActif.ContainsKey(symboleParActif.Value))
                {
                    prix = dictionnairePrixParActif[symboleParActif.Value];
                }

                valeurTotalePatrimoine += quantite * prix;
            }

            valeurTotalePatrimoine = Math.Round(valeurTotalePatrimoine, 2);
        }

        public async Task<string> GetValeurPatrimoineCourant()
        {
            return valeurTotalePatrimoine.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
        }

        public void CalculerVariationPrix()
        {
            this.CalculerQuantiteTotaleInvestit();
            variationPrix = ((valeurTotalePatrimoine - quantiteTotaleInvestit) / quantiteTotaleInvestit);
        }

        public string GetVariationPrix()
        {
            if (variationPrix > 0)
            {
                return "↗ " + variationPrix.ToString("P2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
            }
            else if (variationPrix < 0)
            {
                return "↘" + variationPrix.ToString("P2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
            }
            else
            {
                return variationPrix.ToString("P2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
            }
        }

        public List<ChartLinePoint> GetQuantiteInvestitParDate()
        {
            Dictionary<DateTime, double> quantiteParDate = _IPatrimoine.GetQuantiteInvestitParDate();
            return quantiteParDate.Select(kvp => new ChartLinePoint(kvp.Key.ToString("dd-MM-yy"), (decimal)kvp.Value)).ToList();
        }

        public List<ChartLinePoint> GetValeurPatrimoineParDate()
        {
            Dictionary<DateTime, double> valeurParDate = _IPatrimoine.GetValeurPatrimoineParDate();
            return valeurParDate.Select(kvp => new ChartLinePoint(kvp.Key.ToString("dd-MM-yy"), (decimal)kvp.Value)).ToList();
        }
    }
}
