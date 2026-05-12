namespace Investissement_WebClient.Application.Services.TradeRepublicApi
{
    public interface ITradeRepublicApiService
    {
        Task<(string message, int codeHTTP)> GetSms();

        Task<(string message, int codeHTTP)> ConfirmSms(string codeSms);

        Task<int> ChargerTransactions();
    }
}
