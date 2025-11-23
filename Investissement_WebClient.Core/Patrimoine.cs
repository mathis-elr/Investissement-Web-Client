using Investissement_WebClient.Data.Repository.Interfaces;
using System.Diagnostics;

namespace Investissement_WebClient.Core
{
    public class Patrimoine
    {
        private readonly IPatrimoineSQLite _IPatrimoine;
        private readonly IMarketDataService _IMarketDataService;
        public Patrimoine(IPatrimoineSQLite iPatrimoine, IMarketDataService iMarketDataService) 
        { 
            _IPatrimoine = iPatrimoine;
            _IMarketDataService = iMarketDataService;
        }

        public async Task<double> GetValeurPatrimoineCourant()
        {
            double valeurTotalePatrimoine = 0;
            Dictionary<string, double> dictionnaireQuantiteParActif = _IPatrimoine.ReadQuantiteInvestitParActif();
            Dictionary<string, double> dictionnairePrixParActif = await _IMarketDataService.GetPrixActuelAsync(_IPatrimoine.GetSymboles());

            foreach (KeyValuePair<string, string> symboleParActif in _IPatrimoine.GetSymboleParActif())
            {
                double quantite = dictionnaireQuantiteParActif[symboleParActif.Key];

                double prix = 0;
                if (dictionnairePrixParActif.ContainsKey(symboleParActif.Value))
                {
                    prix = dictionnairePrixParActif[symboleParActif.Value];
                }

                valeurTotalePatrimoine += quantite * prix;
            }
            return Math.Round(valeurTotalePatrimoine, 2);
        }

        public double GetVariationPrix()
        {
            return 0;
        }
    }
}
