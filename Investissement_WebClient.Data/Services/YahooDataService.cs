using System.Diagnostics;
using System.Net.Http.Json;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using YahooFinanceApi;

namespace Investissement_WebClient.Data.Services
{
    public class YahooDataService : IYahooDataService
    {
        private readonly Dictionary<string, string> tickersFixes = new()
        {
            { "FR0013416716", "GLDA.DE" },
            { "XF000BTC0017", "BTC-EUR" }, 
            { "XF000ETH0019", "ETH-EUR" }, 
            { "XF000SOL0012", "SOL-EUR" }, 
        };
        
        public async Task<Dictionary<string, decimal>> GetPrixActuelAsync(IEnumerable<string> symboles)
        {
            var dictionnairePrix = new Dictionary<string, decimal>();
            
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
                        dictionnairePrix[symbole] = (decimal)Math.Round(data.RegularMarketPrice, 2);
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

        public async Task<string?> GetTickerByIsinAsync(string isin)
        {
            if (tickersFixes.TryGetValue(isin, out var manualTicker))
            {
                return manualTicker;
            }

            // Sinon, on lance la recherche HttpClient vue précédemment...
            return await SearchTickerOnYahoo(isin);
        }

        private async Task<string?> SearchTickerOnYahoo(string isin)
        {
            using (HttpClient client = new HttpClient())
            {
                // Yahoo bloque souvent les requêtes sans User-Agent
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                string url = $"https://query2.finance.yahoo.com/v1/finance/search?q={isin}";
                var response = await client.GetAsync(url);
        
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<YahooSearchResultDto>();
                    
                    var quote = json?.Quotes?.FirstOrDefault();
                    return quote?.Ticker; 
                }
            }
            return null;
        }
    }
}
