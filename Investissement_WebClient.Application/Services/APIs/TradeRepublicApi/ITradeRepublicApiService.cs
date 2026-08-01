using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Application.Services.API.TradeRepublicApi
{
    public interface ITradeRepublicApiService
    {
        Task<(int, string)> GetSms(int userId);

        Task<string> ConfirmSms(string codeSms);

        Task<bool> ChargerTransactions(int userId);

        Task<TradeRepublicAccesVM?> GetTradeRepublicAcces(int userId);

        Task SaveAcces(TradeRepublicAccesVM accesDto, int userId);
    }
}
