namespace Investissement_WebClient.Core.Modeles.DTO;

public class DatasTrDto
{
    public List<Transaction> Transactions { get; init; } = new();
    
    public List<FluxTradeRepublic> FluxBancaires { get; init; } = new();
}