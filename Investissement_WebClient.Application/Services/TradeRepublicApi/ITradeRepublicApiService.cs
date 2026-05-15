using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Application.Services.TradeRepublicApi
{
    public interface ITradeRepublicApiService
    {
        Task<string> GetSms(int userId);

        Task<string> ConfirmSms(string codeSms);

        Task<bool> ChargerTransactions(int userId);

        Task SaveAcces(TradeRepublicAccesVM accesDto, int userId);
    }
}
