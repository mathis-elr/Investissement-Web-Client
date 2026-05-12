using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.YahooFinance;

public class YahooTickerApiResponse
{
    [JsonPropertyName("symbol")]
    public string Ticker { get; set; }
}