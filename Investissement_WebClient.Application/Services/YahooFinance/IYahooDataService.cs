namespace Investissement_WebClient.Application.Services.YahooFinance;

public interface IYahooDataService
{
    Task<Dictionary<string, decimal>> GetPrixActuelAsync(IEnumerable<string> symboles);
    
    Task<string?> GetTickerByIsinAsync(string isin);
}