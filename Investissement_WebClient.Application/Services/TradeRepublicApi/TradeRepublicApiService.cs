using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.YahooFinanceApi;
using Investissement_WebClient.Domain.Configurations;

namespace Investissement_WebClient.Application.Services.TradeRepublicApi
{
    public class TradeRepublicApiService : ITradeRepublicApiService
    {
        private readonly IFluxInvestissementService _fluxInvestissementService;
        private readonly IYahooFinanceApiService _yahooFinanceApiService;


        private readonly string API_KEY_KEY =  TradeRepublicApiConfiguration.Key;

        private readonly string API_KEY_VALUE = TradeRepublicApiConfiguration.Value;

        private readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri(TradeRepublicApiConfiguration.BaseUri),
            Timeout = TimeSpan.FromSeconds(30) // Important car Selenium peut être lent
        };

        public TradeRepublicApiService(IFluxInvestissementService fluxInvestissementService,
                                       IYahooFinanceApiService yahooFinanceApiService)
        {
            _fluxInvestissementService = fluxInvestissementService;
            _yahooFinanceApiService = yahooFinanceApiService;

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

                var responseBody = await response.Content.ReadFromJsonAsync<TradeRepublicFluxApiResponse>(options) ?? throw new Exception("L'API a renvoyé un corps vide.");
                
                foreach (var transaction in responseBody.Transactions)
                {
                    transaction.Ticker = await _yahooFinanceApiService.GetTickerByIsinAsync(transaction.ISIN!);
                }
                
                await _fluxInvestissementService.MapperTransactions(responseBody.Transactions);

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
