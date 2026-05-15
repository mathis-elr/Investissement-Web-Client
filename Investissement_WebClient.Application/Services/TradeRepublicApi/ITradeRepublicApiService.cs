namespace Investissement_WebClient.Application.Services.TradeRepublicApi
{
    public interface ITradeRepublicApiService
    {
        Task<string> GetSms();

        Task<string> ConfirmSms(string codeSms);

        Task<bool> ChargerTransactions(int userId);
    }
}
