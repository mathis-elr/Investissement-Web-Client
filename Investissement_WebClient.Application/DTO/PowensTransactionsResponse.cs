using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.DTO;

public class PowensTransactionsResponse
{
    [JsonPropertyName("transactions")]
    public List<FluxCreditCoopDto>? Transactions { get; set; }
}