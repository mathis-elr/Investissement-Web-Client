using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse;

public class PowensTransactionsApiResponse
{
    [JsonPropertyName("transactions")]
    public List<FluxCreditCoopApiResponse>? Transactions { get; set; }
}