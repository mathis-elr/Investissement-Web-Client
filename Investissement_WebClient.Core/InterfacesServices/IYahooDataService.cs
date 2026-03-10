namespace Investissement_WebClient.Core.InterfacesServices;

public interface IYahooDataService
{
    Task<Dictionary<string, decimal>> GetPrixActuelAsync(List<string> symboles);
}