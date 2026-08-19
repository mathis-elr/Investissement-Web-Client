using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensConnectorApiResponse
    {
        [JsonPropertyName("name")]
        public string? NomBanque { get; set; }
    }
}
