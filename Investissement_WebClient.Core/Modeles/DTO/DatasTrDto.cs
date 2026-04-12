namespace Investissement_WebClient.Core.Modeles.DTO;

public class DatasTrDto
{
    public List<Transaction> Transactions { get; init; } = new();
    
    public List<FluxBancaire> FluxBancaires { get; init; } = new();
}