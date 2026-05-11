using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse
{
    public class PowensComptesApiResponse
    {
        [JsonPropertyName("accounts")]
        public List<PowensTypeCompteApiResponse> Comptes { get; set; } = [];
    }
}
