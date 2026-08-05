using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Interfaces.APIs
{
    public interface IYahooFinanceApiService
    {
        Task<Dictionary<string, decimal>> GetPrixActuelAsync(IEnumerable<string> symboles);

        Task<Dictionary<LapsTemps, decimal>> GetPrixHistorique(string ticker);

        Task<string?> GetTickerByIsinAsync(string isin);
    }
}

