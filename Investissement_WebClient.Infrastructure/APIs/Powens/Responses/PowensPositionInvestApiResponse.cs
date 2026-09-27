using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensPositionInvestApiResponse
    {
        [JsonPropertyName("code_type")]
        public string CodeType { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public decimal Quantite { get; set; }

        [JsonPropertyName("unitprice")]
        public decimal PrixAchat { get; set; }

        [JsonPropertyName("unitvalue")]
        public decimal PrixCourant { get; set; }

        [JsonPropertyName("vdate")]
        public DateTime DatePrixCourant { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime DateDerniereMAJ { get; set; }
    }
}
