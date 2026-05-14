using Investissement_WebClient.Application.ApiResponse.YahooFinance;
using Investissement_WebClient.Domain.Configurations;
using System.Net.Http.Json;
using YahooFinanceApi;

namespace Investissement_WebClient.Application.Services.YahooFinanceApi
{
    public class YahooFinanceApiService : IYahooFinanceApiService
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
            catch (Exception)
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

            return await SearchTickerOnYahoo(isin);
        }

        private async Task<string?> SearchTickerOnYahoo(string isin)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                string url = YahooFinanceApiConfiguration.BaseUri + isin;
                var response = await client.GetAsync(url);
        
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<YahooSearchApiResponse>();
                    
                    var quote = json?.Quotes?.FirstOrDefault();
                    return quote?.Ticker; 
                }
            }
            return null;
        }
    }
}
