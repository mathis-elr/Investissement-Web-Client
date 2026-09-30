using Investissement_WebClient.Infrastructure.APIs.Powens.Converters;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensConnectionApiResponse
    {
        [JsonPropertyName("id_connector")]
        public int IdConnector { get; set; }

        [JsonPropertyName("state")]
        public string? PowensState { get; set; }

        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(PowensDateTimeConverter))]
        public DateTime? LastUpdate { get; set; }
    }
}
