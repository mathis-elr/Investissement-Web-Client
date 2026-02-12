namespace Investissement_WebClient.Core.InterfacesServices;

public interface IYahooDataService
{
    Task<Dictionary<string, double>> GetPrixActuelAsync(List<string> symboles);
}