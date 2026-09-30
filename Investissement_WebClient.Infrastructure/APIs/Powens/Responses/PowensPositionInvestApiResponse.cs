using Investissement_WebClient.Infrastructure.APIs.Powens.Converters;
using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.Powens.Responses
{
    public class PowensPositionInvestApiResponse
    {
        [JsonPropertyName("code_type")]
        public string? CodeType { get; set; } 

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public decimal Quantite { get; set; }

        [JsonPropertyName("unitprice")]
        public decimal PrixAchat { get; set; }

        [JsonPropertyName("unitvalue")]
        public decimal PrixCourant { get; set; }

        [JsonPropertyName("vdate")]
        [JsonConverter(typeof(PowensDateTimeConverter))]
        public DateTime? DatePrixCourant { get; set; }

        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(PowensDateTimeConverter))]
        public DateTime? DateDerniereMAJ { get; set; }
    }
}
