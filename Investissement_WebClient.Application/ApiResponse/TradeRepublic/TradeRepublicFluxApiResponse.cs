using Investissement_WebClient.Domain.Modeles;
using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.TradeRepublic;

public class TradeRepublicFluxApiResponse 
{
    [JsonPropertyName("Transactions")]
    public List<FluxInvestissement> Transactions { get; init; } = new();
}