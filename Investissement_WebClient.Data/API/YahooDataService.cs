using System.Diagnostics;
using YahooFinanceApi;

namespace Investissement_WebClient.Data.Repository.Services
{
    public class YahooDataService
    {
        public async Task<Dictionary<string, double>> GetPrixActuelAsync(List<string> symboles)
        {
            var dictionnairePrix = new Dictionary<string, double>();

            // S'il n'y a rien à demander, on retourne le dictionnaire vide
            if (symboles == null || !symboles.Any())
            {
                return dictionnairePrix;
            }

            try
            {
                //UN SEUL appel pour TOUS les symboles en même temps.
                IReadOnlyDictionary<string, Security> resultats =
                    await Yahoo.Symbols(symboles.ToArray()).QueryAsync();

                // 2. On traite les résultats
                foreach (var symbole in symboles)
                {
                    if (resultats.TryGetValue(symbole, out Security data))
                    {
                        // "RegularMarketPrice" est le prix actuel (ou le dernier prix de clôture)
                        dictionnairePrix[symbole] = Math.Round((double)data.RegularMarketPrice, 2);
                    }
                    else
                    {
                        Console.WriteLine($"AVERTISSEMENT (YahooApi): Symbole non trouvé {symbole}");
                        dictionnairePrix[symbole] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERREUR recuperation prix actifs API yahoo : {ex.Message}");
                throw new Exception($"ERREUR de connexion pour la recuperation prix");
            }

            return dictionnairePrix;
        }
    }
}
