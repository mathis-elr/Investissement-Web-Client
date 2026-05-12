namespace Investissement_WebClient.Application.Services.YahooFinanceApi;

public interface IYahooFinanceApiService
{
    Task<Dictionary<string, decimal>> GetPrixActuelAsync(IEnumerable<string> symboles);
    
    Task<string?> GetTickerByIsinAsync(string isin);
}