using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.YahooFinance;

public class YahooSearchApiResponse
{
    [JsonPropertyName("quotes")]
    public List<YahooTickerApiResponse> Quotes { get; set; } = [];
}