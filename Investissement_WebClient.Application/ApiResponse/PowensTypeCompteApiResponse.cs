using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse
{
    public class PowensTypeCompteApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
