using System.Text.Json.Serialization;

namespace Investissement_WebClient.Core.Modeles.DTO;

public class YahooResultTickerDto
{
    [JsonPropertyName("symbol")]
    public string Ticker { get; set; }
}