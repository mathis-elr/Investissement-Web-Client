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
        public string? RawLastUpdate { get; set; }

        [JsonIgnore]
        public DateTime? LastUpdate => DateTime.TryParseExact(
            RawLastUpdate,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var dt) ? dt : null;
    }
}
