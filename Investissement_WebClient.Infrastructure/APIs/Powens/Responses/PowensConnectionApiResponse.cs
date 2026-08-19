using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensConnectionApiResponse
    {
        [JsonPropertyName("id_connector")]
        public int IdConnector { get; set; }
    }
}
