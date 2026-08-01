using Investissement_WebClient.Application.ApiResponse.YahooFinance;
using Investissement_WebClient.Domain.Configurations;
using Investissement_WebClient.Domain.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using YahooFinanceApi;

namespace Investissement_WebClient.Application.Services.API.YahooFinanceApi
{
    public class YahooFinanceApiService : IYahooFinanceApiService
    {
        private readonly string _baseUri = YahooFinanceApiConfiguration.BaseUri;
        private readonly string _searchEndPoint = YahooFinanceApiConfiguration.SearchEndPoint;

        private readonly Dictionary<string, string> tickersFixes = new()
        {
            { "FR0013416716", "GLDA.DE" },
            { "XF000BTC0017", "BTC-EUR" }, 
            { "XF000ETH0019", "ETH-EUR" }, 
            { "XF000SOL0012", "SOL-EUR" }, 
        };

        public async Task<Dictionary<string, decimal>> GetPrixActuelAsync(IEnumerable<string> tickers)
        {
            var dictionnairePrix = new Dictionary<string, decimal>();
            
            if (!tickers.Any())
                return dictionnairePrix;

            try
            {
                //UN SEUL appel pour TOUS les tickers en même temps.
                IReadOnlyDictionary<string, Security> resultats =
                    await Yahoo.Symbols(tickers.ToArray()).QueryAsync();
                
                foreach (var ticker in tickers)
                {
                    if (resultats.TryGetValue(ticker, out var data))
                    {
                        dictionnairePrix[ticker] = (decimal)Math.Round(data.RegularMarketPrice, 2);
                    }
                    else
                    {
                        Console.WriteLine($"AVERTISSEMENT (YahooApi): ticker non trouvé {ticker}");
                        throw new Exception("ERREUR de ticker");
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

        public async Task<Dictionary<LapsTemps, decimal>> GetPrixHistorique(string ticker)
        {
            var dictionnairePrix = new Dictionary<LapsTemps, decimal>();

            // On demande directement 1 an d'historique (range=1y) avec un intervalle d'un jour (1d)
            string url = $"https://query1.finance.yahoo.com/v8/finance/chart/{ticker}?range=1y&interval=1d";

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                using var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return dictionnairePrix;

                // On parse le gros JSON de Yahoo
                using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                var chart = doc.RootElement.GetProperty("chart");

                if (chart.TryGetProperty("result", out var resultList) && resultList.GetArrayLength() > 0)
                {
                    var firstResult = resultList[0];
                    var historique = new List<(DateTime Date, decimal Price)>();

                    if (firstResult.TryGetProperty("timestamp", out var timestamps))
                    {
                        var indicators = firstResult.GetProperty("indicators");
                        var quote = indicators.GetProperty("quote");

                        if (quote.GetArrayLength() > 0 && quote[0].TryGetProperty("close", out var closes))
                        {
                            for (int i = 0; i < timestamps.GetArrayLength(); i++)
                            {
                                if (i < closes.GetArrayLength() && closes[i].ValueKind != JsonValueKind.Null)
                                {
                                    var dateBougie = DateTimeOffset.FromUnixTimeSeconds(timestamps[i].GetInt64()).DateTime.Date;
                                    historique.Add((dateBougie, closes[i].GetDecimal()));
                                }
                            }
                        }
                    }

                    foreach (LapsTemps periode in Enum.GetValues(typeof(LapsTemps)))
                    {
                        if (periode == LapsTemps.All) continue;

                        DateTime dateCible = DateTime.Today.AddDays(-(int)periode);

                        if (historique.Any())
                        {
                            var proche = historique
                                .OrderBy(x => Math.Abs((x.Date - dateCible).TotalDays))
                                .FirstOrDefault();

                            dictionnairePrix[periode] = proche.Price;
                        }
                        else
                        {
                            firstResult.TryGetProperty("meta", out var meta);
                            dictionnairePrix[periode] = meta.TryGetProperty("regularMarketPrice", out var marketPrice) ? marketPrice.GetDecimal() : 0;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Yahoo Chart pour {ticker}: {ex.Message}");
                // En cas de pépin, on initialise à 0 pour ne pas faire crasher ton dashboard
                foreach (LapsTemps p in Enum.GetValues(typeof(LapsTemps))) dictionnairePrix[p] = 0;
            }


            foreach (var a in dictionnairePrix)
            {
                Console.WriteLine(ticker + " => " + a.Key + " : " + a.Value);
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

                string url = _baseUri + _searchEndPoint + isin;
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
