namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IMarketDataService
    {
        Task<Dictionary<string,double>> GetPrixActuelAsync(List<string> symboleActif);
    }
}
