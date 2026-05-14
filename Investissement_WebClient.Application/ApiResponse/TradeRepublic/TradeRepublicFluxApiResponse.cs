using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.TradeRepublic;

public class TradeRepublicFluxApiResponse 
{
    [JsonPropertyName("Transactions")]
    public List<TradeRepublicUnFluxApiResponse> Transactions { get; init; } = new();
}