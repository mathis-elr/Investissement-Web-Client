using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.DTO;

public class YahooSearchResultDto
{
    [JsonPropertyName("quotes")]
    public List<YahooResultTickerDto> Quotes { get; set; }
}