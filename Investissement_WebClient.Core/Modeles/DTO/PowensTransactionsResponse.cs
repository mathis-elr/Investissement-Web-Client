using System.Text.Json.Serialization;

namespace Investissement_WebClient.Core.Modeles.DTO;

public class PowensTransactionsResponse
{
    [JsonPropertyName("transactions")]
    public List<FluxCreditCoopDto>? Transactions { get; set; }
}