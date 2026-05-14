using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Domain.Configurations;

namespace Investissement_WebClient.Application.Services.TradeRepublicApi
{
    public class TradeRepublicApiService : ITradeRepublicApiService
    {
        private readonly IFluxInvestissementService _fluxInvestissementService;


        private readonly string _cleeApiKey =  TradeRepublicApiConfiguration.CleeApiKey;
        private readonly string _cleeApiValue = TradeRepublicApiConfiguration.CleeApiValue;

        private readonly string _numTelKey = TradeRepublicApiConfiguration.NumTelKey;
        private readonly string _numTelValue = TradeRepublicApiConfiguration.NumTelValue;

        private readonly string _pinKey = TradeRepublicApiConfiguration.PinKey;
        private readonly string _pinValue = TradeRepublicApiConfiguration.PinValue;

        private readonly string _dernierIdEnregistreKey = TradeRepublicApiConfiguration.DernierIdEnregistreKey;

        private readonly HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri(TradeRepublicApiConfiguration.BaseUri),
            Timeout = TimeSpan.FromSeconds(30) // Important car Selenium peut être lent
        };

        public TradeRepublicApiService(IFluxInvestissementService fluxInvestissementService)
        {
            _fluxInvestissementService = fluxInvestissementService;

            if (!Client.DefaultRequestHeaders.Contains(_cleeApiKey))
            {
                Client.DefaultRequestHeaders.Add(_cleeApiKey, _cleeApiValue);
            }
        }

        public async Task<(string message, int codeHTTP)> GetSms()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "auth/request-sms");

                request.Headers.Add(_numTelKey, _numTelValue);
                request.Headers.Add(_pinKey, _pinValue);

                var response = await Client.SendAsync(request);

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
                var dernierIdEnregistreValue = await _fluxInvestissementService.GetDernierFluxEnregistre();
                var request = new HttpRequestMessage(HttpMethod.Get, "datas");

                if (!string.IsNullOrEmpty(dernierIdEnregistreValue))
                    request.Headers.Add(_dernierIdEnregistreKey, dernierIdEnregistreValue);

                var response = await Client.SendAsync(request);

                int codeStatus = (int)response.StatusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var strcontent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(strcontent);

                var responseBody = await response.Content.ReadFromJsonAsync<TradeRepublicFluxApiResponse>(options) ?? throw new Exception("L'API a renvoyé un corps vide.");
                
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
