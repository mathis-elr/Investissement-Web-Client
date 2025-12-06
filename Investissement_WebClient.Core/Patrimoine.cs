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
        private Dictionary<string, double> dictionnaireQuantiteEURParActif { get; set; } = new Dictionary<string, double>();

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

        public async Task CalculerQuantiteEurParActif()
        {
            await this.LoadPrixParActif();
                foreach (KeyValuePair<string, string> symboleParActif in _IPatrimoine.GetSymboleParActif())
                {
                    double quantite = dictionnaireQuantiteParActif[symboleParActif.Key].Item1;

                    double prix = 0;
                    if (dictionnairePrixParActif.ContainsKey(symboleParActif.Value))
                    {
                        prix = dictionnairePrixParActif[symboleParActif.Value];
                    }

                    dictionnaireQuantiteEURParActif[symboleParActif.Key] = Math.Round(quantite * prix, 2);
                }
        }

        public void CalculerValeurPatrimoineCourant()
        {
            valeurTotalePatrimoine = dictionnaireQuantiteEURParActif.Sum(actif => actif.Value);
            valeurTotalePatrimoine = Math.Round(valeurTotalePatrimoine, 2);
            Console.WriteLine(valeurTotalePatrimoine);
        }

        public string GetValeurPatrimoineCourant()
        {
            return valeurTotalePatrimoine.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
        }

        public void CalculerVariationPrix()
        {
            this.CalculerQuantiteTotaleInvestit();
            variationPrix = ((valeurTotalePatrimoine - quantiteTotaleInvestit) / quantiteTotaleInvestit);
        }

        public Dictionary<string,double> CalculerProportionParActif()
        {
            Dictionary<string, double> proportionParActif = new Dictionary<string, double>();
            foreach (KeyValuePair<string,double> prixParActif in this.dictionnaireQuantiteEURParActif)
            {
                double proportion = prixParActif.Value / valeurTotalePatrimoine * 100;
                proportionParActif.Add(prixParActif.Key, Math.Round(proportion,2));
            }
            return proportionParActif;
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

        public List<ChartPieProportionParActif> GetProportionParActif()
        {
            Dictionary<string, double> proporionParActif = this.CalculerProportionParActif();
            return proporionParActif.Select(kvp => new ChartPieProportionParActif(kvp.Key, (decimal)kvp.Value)).ToList();
        }
    }
}
