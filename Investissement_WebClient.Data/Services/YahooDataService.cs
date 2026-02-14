using System.Diagnostics;
using Investissement_WebClient.Core.InterfacesServices;
using YahooFinanceApi;

namespace Investissement_WebClient.Data.Services
{
    public class YahooDataService : IYahooDataService
    {
        public async Task<Dictionary<string, double>> GetPrixActuelAsync(List<string> symboles)
        {
            var dictionnairePrix = new Dictionary<string, double>();
            
            if (symboles == null || !symboles.Any())
            {
                return dictionnairePrix;
            }

            try
            {
                //UN SEUL appel pour TOUS les symboles en même temps.
                IReadOnlyDictionary<string, Security> resultats =
                    await Yahoo.Symbols(symboles.ToArray()).QueryAsync();
                
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
                        throw new Exception("ERREUR de symbole");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERREUR internet, impossible de recuperer le prix des actifs avec l'API yahoo");
                throw new Exception($"Echec du chargement du patrimoine, connectez-vous à internet pour visualiser votre patrimoine");
            }

            return dictionnairePrix;
        }
    }
}
