using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.DTO;

public class YahooResultTickerDto
{
    [JsonPropertyName("symbol")]
    public string Ticker { get; set; }
}