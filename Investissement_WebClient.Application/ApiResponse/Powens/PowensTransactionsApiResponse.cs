using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.Powens;

public class PowensTransactionsApiResponse
{
    [JsonPropertyName("transactions")]
    public List<PowensFluxApiResponse> Transactions { get; set; } = [];
}