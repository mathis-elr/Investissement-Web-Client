using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Modeles;
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
        private Dictionary<string, double> dictionnaireValeurPatrimoineParActif { get; set; } 

        public Patrimoine(IPatrimoineSQLite iPatrimoine, IMarketDataService iMarketDataService)
        {
            _IPatrimoine = iPatrimoine;
            _IMarketDataService = iMarketDataService;

            dictionnaireQuantiteParActif = _IPatrimoine.GetQuantiteInvestitParActif();
            dictionnairePrixParActif = new Dictionary<string, double>();
            dictionnaireValeurPatrimoineParActif = new Dictionary<string, double>();
        }
        
        /*-- Valeur Patrimoine Total --*/
        public async Task<string> GetValeurPatrimoineCourant()
        {
            await CalculerValeurPatrimoineCourant();
            return valeurTotalePatrimoine.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
        }
        public async Task CalculerValeurPatrimoineCourant()
        {
            await CalculerValeurPatrimoineParActif();
            valeurTotalePatrimoine = dictionnaireValeurPatrimoineParActif.Sum(actif => actif.Value);
            valeurTotalePatrimoine = Math.Round(valeurTotalePatrimoine, 2);
        }
        private async Task CalculerValeurPatrimoineParActif()
        {
            await RecupererPrixParActif();
            foreach (KeyValuePair<string, string> symboleParActif in _IPatrimoine.GetSymboleParActif())
            {
                double quantite = dictionnaireQuantiteParActif[symboleParActif.Key].Item1;

                double prix = 0;
                if (dictionnairePrixParActif.ContainsKey(symboleParActif.Value))
                {
                    prix = dictionnairePrixParActif[symboleParActif.Value];
                }

                dictionnaireValeurPatrimoineParActif[symboleParActif.Key] = Math.Round(quantite * prix, 2);
            }
        }
        private async Task RecupererPrixParActif()
        {
            dictionnairePrixParActif = await _IMarketDataService.GetPrixActuelAsync(_IPatrimoine.GetSymboles());
        }
        
        /*-- Variation de prix (en %) entre la valeur actuelle et la quantite totale investit --*/
        public string GetVariationPrix()
        {
            CalculerVariationPrix();
            
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
        public void CalculerVariationPrix()
        {
            CalculerQuantiteTotaleInvestit();
            variationPrix = ((valeurTotalePatrimoine - quantiteTotaleInvestit) / quantiteTotaleInvestit);
        }
        private void CalculerQuantiteTotaleInvestit()
        {
            quantiteTotaleInvestit = dictionnaireQuantiteParActif.Sum(actif => actif.Value.Item2);
        }
        
        /*-- Données pour les deux Line du graphique --*/
        public List<ChartsLinesPrix> GetQuantiteInvestitParDate()
        {
            Dictionary<DateTime, double> quantiteParDate = _IPatrimoine.GetQuantiteInvestitParDate();
            return quantiteParDate.Select(kvp => new ChartsLinesPrix(kvp.Key.ToString("dd-MM-yy"), (decimal)kvp.Value)).ToList();
        }

        public List<ChartsLinesPrix> GetValeurPatrimoineParDate()
        {
            Dictionary<DateTime, double> valeurParDate = _IPatrimoine.GetValeurPatrimoineParDate();
            return valeurParDate.Select(kvp => new ChartsLinesPrix(kvp.Key.ToString("dd-MM-yy"), (decimal)kvp.Value)).ToList();
        }
        
        /*-- Données pour le diagramme pie proportion par actif--*/
        public List<ChartPieProportionParActif> GetProportionParActif()
        {
            Dictionary<string, double> proporionParActif = CalculerProportionParActif();
            return proporionParActif.Select(kvp => new ChartPieProportionParActif(kvp.Key, (decimal)kvp.Value)).ToList();
        }
        public Dictionary<string,double> CalculerProportionParActif()
        {
            Dictionary<string, double> proportionParActif = new Dictionary<string, double>();
            foreach (KeyValuePair<string,double> prixParActif in dictionnaireValeurPatrimoineParActif)
            {
                double proportion = prixParActif.Value / valeurTotalePatrimoine * 100;
                proportionParActif.Add(prixParActif.Key, Math.Round(proportion,2));
            }
            return proportionParActif;
        }
    }
}
