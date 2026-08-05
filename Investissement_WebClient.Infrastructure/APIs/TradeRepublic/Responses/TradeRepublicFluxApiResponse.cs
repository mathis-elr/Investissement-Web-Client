using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.TradeRepublic.Responses
{
    public class TradeRepublicFluxApiResponse
    {
        [JsonPropertyName("Transactions")]
        public List<TradeRepublicUnFluxApiResponse> Transactions { get; init; } = [];
    }
}

