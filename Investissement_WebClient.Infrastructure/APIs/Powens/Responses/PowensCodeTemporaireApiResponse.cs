using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensCodeTemporaireApiResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
