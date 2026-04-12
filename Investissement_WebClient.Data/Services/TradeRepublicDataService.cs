using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Data.Services
{
    public class TradeRepublicDataService : ITradeRepublicDataService
    {
        private readonly IInvestissementService _investissementService;
        
        private readonly IYahooDataService _yahooDataService;

        private readonly string API_KEY_KEY =  "X-API-KEY";

        private readonly string API_KEY_VALUE = "franchement...lesapi?cesttopissime";

        private readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri("http://89.168.42.226:5000/"),
            Timeout = TimeSpan.FromSeconds(30) // Important car Selenium peut être lent
        };

        public TradeRepublicDataService(IInvestissementService investissementService, IYahooDataService yahooDataService)
        {
            _investissementService = investissementService;
            _yahooDataService = yahooDataService;

            if (!Client.DefaultRequestHeaders.Contains(API_KEY_KEY))
            {
                Client.DefaultRequestHeaders.Add(API_KEY_KEY, API_KEY_VALUE);
            }
        }

        public async Task<(string message, int codeHTTP)> GetSms()
        {
            try
            {
                var response = await Client.PostAsync("auth/request-sms", null);

                int codeStatus = (int)response.StatusCode;

                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var retour = doc.RootElement;

                string message = string.Empty;
                if (retour.TryGetProperty("message", out var messageJson))
                {
                    message = messageJson.GetString() ?? string.Empty;
                }

                if (retour.TryGetProperty("coutdouwn", out var coutdouwnJson))
                {
                    string? Coutdouwn = coutdouwnJson.GetString();
                    message += Coutdouwn != null ? $"{Coutdouwn}s restantes." : string.Empty;
                }

                return (message, codeStatus);
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException ("Impossible de contacter l'API");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur innatendu lors du requet sms" + ex.Message);
                throw new Exception ($"Erreur inattendue lors de la demande d'envoi sms");
            }
        }

        public async Task<(string message, int codeHTTP)> ConfirmSms(string codeSms)
        {
            try
            {
                if (codeSms.Length != 4)
                    throw new Exception("Format du code invalide, 4 chiffres requis");

                var body = new { code = codeSms };
                var response = await Client.PostAsJsonAsync("auth/confirm-sms", body);

                int codeStatus = (int)response.StatusCode;

                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var retour = doc.RootElement;

                string message = string.Empty;
                if (retour.TryGetProperty("message", out var messageJson))
                {
                    message = messageJson.GetString() ?? string.Empty;
                }

                return (message, codeStatus);
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException("Impossible de contacter l'API");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur innatendu lors de la confirmation sms" + ex.Message);
                throw new Exception($"Erreur inattendue lors de la confirmation du sms");
            }
        }

        public async Task<int> ChargerTransactions()
        {
            try
            {
                var response = await Client.GetAsync("datas");

                int codeStatus = (int)response.StatusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                var responseBody = await response.Content.ReadFromJsonAsync<DatasTrDto>(options);

                if (responseBody == null)
                {
                    throw new Exception("L'API a renvoyé un corps vide.");
                }

                foreach (var transaction in responseBody.Transactions)
                {
                    transaction.Ticker = await _yahooDataService.GetTickerByIsinAsync(transaction.ISIN);
                }
                
                await _investissementService.AddTransactionsRange(responseBody.Transactions ?? new());
                await _investissementService.AddFluxBancairesRange(responseBody.FluxBancaires ?? new());

                return codeStatus;
            }
            catch (HttpRequestException)
            {
                throw new HttpRequestException("Impossible de contacter l'API");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur innatendu lors de la recuperation des transactions" + ex.Message);
                throw new Exception($"Erreur inattendue lors de la récupération des transactions");
            }
        }
    }
}
