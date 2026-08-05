using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensComptesApiResponse
    {
        [JsonPropertyName("accounts")]
        public List<PowensTypeCompteApiResponse> Comptes { get; set; } = [];
    }
}
