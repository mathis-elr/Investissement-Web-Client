using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.DTO;

public class DatasTrDto 
{
    public List<Transaction> Transactions { get; init; } = new();
    
    public List<FluxTradeRepublic> FluxBancaires { get; init; } = new();
}