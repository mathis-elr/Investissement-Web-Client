using Investissement_WebClient.Infrastructure.APIs.TradeRepublic.Responses;
using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Application.DTO.Auth;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Investissement_WebClient.Infrastructure.APIs.TradeRepublic
{
    public class TradeRepublicApiService : ITradeRepublicApiService
    {
        private readonly ITradeRepublicAccesRepository _tradeRepublicAccesRepository;
        private readonly IFluxInvestissementService _fluxInvestissementService;
        private readonly TradeRepublicApiOptions _options;
        private readonly ICryptService _encryptService;
        private readonly HttpClient _httpClient;

        public TradeRepublicApiService(ITradeRepublicAccesRepository tradeRepublicAccesRepository,
                                       IFluxInvestissementService fluxInvestissementService,
                                       IOptions<TradeRepublicApiOptions> options,
                                       ICryptService encryptService, 
                                       HttpClient httpClient)
        {
            _tradeRepublicAccesRepository = tradeRepublicAccesRepository;   
            _fluxInvestissementService = fluxInvestissementService;
            _encryptService = encryptService;
            _options = options.Value;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_options.BaseUri);
            _httpClient.Timeout = TimeSpan.FromSeconds(120);

            if (!_httpClient.DefaultRequestHeaders.Contains(_options.CleeApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(_options.CleeApiKey, _options.CleeApiValue);
            }
        }

        public async Task<(int, string)> GetSms(int userId)
        {
            try
            {
                var accesTR = await GetTradeRepublicAcces(userId) ?? throw new Exception("Identifiants Trade Republic manquants");

                var request = new HttpRequestMessage(HttpMethod.Post, _options.RequestSmsEndPoint);

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
                var response = await _httpClient.PostAsJsonAsync(_options.ConfirmSmsEndPoint, body);

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
                var request = new HttpRequestMessage(HttpMethod.Get, _options.DatasEndPoint);

                if (!string.IsNullOrEmpty(dernierIdEnregistreValue))
                    request.Headers.Add(_options.DernierIdEnregistreKey, dernierIdEnregistreValue);

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
                    var fluxImport = responseBody.Transactions
                        .Where(t =>
                            t.Id != null &&
                            t.Date.HasValue &&
                            t.Prix.HasValue &&
                            t.Quantite.HasValue &&
                            t.Actif != null &&
                            t.ISIN != null)
                        .Select(t => new FluxInvestissementImportDto
                        {
                            Id = t.Id!,
                            Date = t.Date!.Value,
                            Type = (TypeFlux)t.Type!.Value,
                            Prix = t.Prix.Value,
                            Quantite = t.Quantite.Value,
                            Frais = t.Frais,
                            Total = t.Total,
                            ISIN = t.ISIN!,
                            Actif = t.Actif!
                        })
                        .ToList();

                    await _fluxInvestissementService.MapperTransactions(fluxImport,userId);

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

        public async Task<TradeRepublicAccesDto?> GetTradeRepublicAcces(int userId)
        {
            var acces = await _tradeRepublicAccesRepository.GetByUserId(userId);

            var accesDto = acces != null ? new TradeRepublicAccesDto
            {
                NumTel = acces.NumTel,
                Pin = _encryptService.Decrypt(acces.PinCrypte.ToString(), _options.MasterKey)
            } : null;

            return accesDto;
        }

        public async Task SaveAcces(TradeRepublicAccesDto accesDto, int userId)
        {
            var acces = await _tradeRepublicAccesRepository.GetByUserId(userId);

            if (acces != null)
            {
                var numTelEtier = accesDto.NumTel.Replace(" ", "");

                acces.NumTel = numTelEtier;
                acces.PinCrypte = _encryptService.Encrypt(accesDto.Pin, _options.MasterKey);
            }
            else
            {
                var newAcces = new TradeRepublicAcces
                {
                    NumTel = accesDto.NumTel,
                    PinCrypte = _encryptService.Encrypt(accesDto.Pin, _options.MasterKey),
                    UtilisateurId = userId
                };

                await _tradeRepublicAccesRepository.Add(newAcces);
            }
        }
    }
}
