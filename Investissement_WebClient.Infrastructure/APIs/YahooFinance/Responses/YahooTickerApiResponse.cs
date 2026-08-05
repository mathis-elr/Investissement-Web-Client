using System.Text.Json.Serialization;

namespace Investissement_WebClient.Infrastructure.APIs.YahooFinance.Responses
{
    public class YahooTickerApiResponse
    {
        [JsonPropertyName("symbol")]
        public string? Ticker { get; set; }
    }
}

