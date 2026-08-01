using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.Services.Encrypt;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Domain.Configurations;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace Investissement_WebClient.Application.Services.API.TradeRepublicApi
{
    public class TradeRepublicApiService : ITradeRepublicApiService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
        private readonly IFluxInvestissementService _fluxInvestissementService;
        private readonly ICryptService _encryptService;
        private readonly HttpClient _httpClient;

        private readonly string _masterKey = TradeRepublicApiConfiguration.MasterKey;

        private readonly string _cleeApiKey = TradeRepublicApiConfiguration.CleeApiKey;
        private readonly string _cleeApiValue = TradeRepublicApiConfiguration.CleeApiValue;

        private readonly string _numTelKey = TradeRepublicApiConfiguration.NumTelKey;
        private readonly string _pinKey = TradeRepublicApiConfiguration.PinKey;

        private readonly string _dernierIdEnregistreKey = TradeRepublicApiConfiguration.DernierIdEnregistreKey;

        private readonly string _requestSmsEndPoint = TradeRepublicApiConfiguration.RequestSmsEndPoint;
        private readonly string _confirmSmsEndPoint = TradeRepublicApiConfiguration.ConfirmSmsEndPoint;
        private readonly string _datasEndPoint = TradeRepublicApiConfiguration.DatasEndPoint;

        public TradeRepublicApiService(IDbContextFactory<InvestissementDbContext> dbFactory, 
                                       IFluxInvestissementService fluxInvestissementService, 
                                       ICryptService encryptService, 
                                       HttpClient httpClient)
        {
            _dbFactory = dbFactory;
            _fluxInvestissementService = fluxInvestissementService;
            _encryptService = encryptService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(TradeRepublicApiConfiguration.BaseUri);
            _httpClient.Timeout = TimeSpan.FromSeconds(120);

            if (!_httpClient.DefaultRequestHeaders.Contains(_cleeApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(_cleeApiKey, _cleeApiValue);
            }
        }

        public async Task<(int, string)> GetSms(int userId)
        {
            try
            {
                var accesTR = await GetTradeRepublicAcces(userId) ?? throw new Exception("Identifiants Trade Republic manquants");

                var request = new HttpRequestMessage(HttpMethod.Post, _requestSmsEndPoint);

                var jsonPayload = "{\"num-tel\":\"+33" + accesTR.NumTel + "\", \"pin\":\"" + accesTR.Pin + "\"}";
                request.Content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                int codeStatus = (int)response.StatusCode;

                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var retour = doc.RootElement;

                string message = string.Empty;
                if (retour.TryGetProperty("message", out var messageJson))
                {
                    message = messageJson.GetString() ?? string.Empty;
                }

                return (codeStatus, message);
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

        public async Task<string> ConfirmSms(string codeSms)
        {
            try
            {
                if (codeSms.Length != 4)
                    throw new Exception("Format du code invalide, 4 chiffres requis");

                var body = new { code = codeSms };
                var response = await _httpClient.PostAsJsonAsync(_confirmSmsEndPoint, body);

                int codeStatus = (int)response.StatusCode;

                var responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var retour = doc.RootElement;

                string message = string.Empty;
                if (retour.TryGetProperty("message", out var messageJson))
                {
                    message = messageJson.GetString() ?? string.Empty;
                }

                return message;
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

        public async Task<bool> ChargerTransactions(int userId)
        {
            try
            {
                var dernierIdEnregistreValue = await _fluxInvestissementService.GetDernierFluxEnregistre(userId);
                var request = new HttpRequestMessage(HttpMethod.Get, _datasEndPoint);

                if (!string.IsNullOrEmpty(dernierIdEnregistreValue))
                    request.Headers.Add(_dernierIdEnregistreKey, dernierIdEnregistreValue);

                var response = await _httpClient.SendAsync(request);

                int codeStatus = (int)response.StatusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var responseBody = await response.Content.ReadFromJsonAsync<TradeRepublicFluxApiResponse>(options) ?? throw new Exception("L'API a renvoyé un corps vide.");
                
                if(responseBody.Transactions.Count > 0)
                {
                    await _fluxInvestissementService.MapperTransactions(responseBody.Transactions, userId);
                    return true;
                }
                return false;
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

        public async Task<TradeRepublicAccesVM?> GetTradeRepublicAcces(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var acces = await context.TradeRepublicAcces
                .FirstOrDefaultAsync(b => b.UtilisateurId == userId);

            var accesDto = acces != null ? new TradeRepublicAccesVM
            {
                NumTel = acces.NumTel,
                Pin = _encryptService.Decrypt(acces.PinCrypte.ToString(), _masterKey)
            } : null;

            return accesDto;
        }

        public async Task SaveAcces(TradeRepublicAccesVM accesDto, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var acces = await context.TradeRepublicAcces
                .FirstOrDefaultAsync(b => b.UtilisateurId == userId);
           
            if (acces != null)
            {
                var numTelEtier = accesDto.NumTel.Replace(" ", "");

                acces.NumTel = numTelEtier;
                acces.PinCrypte = _encryptService.Encrypt(accesDto.Pin, _masterKey);
            }
            else
            {
                var newAcces = new TradeRepublicAcces
                {
                    NumTel = accesDto.NumTel,
                    PinCrypte = _encryptService.Encrypt(accesDto.Pin, _masterKey),
                    UtilisateurId = userId
                };
                await context.TradeRepublicAcces.AddAsync(newAcces);
            }

            await context.SaveChangesAsync();
        }
    }
}
