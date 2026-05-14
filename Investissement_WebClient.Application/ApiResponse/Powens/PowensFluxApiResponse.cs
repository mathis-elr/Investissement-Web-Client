using System.Text.Json.Serialization;

namespace Investissement_WebClient.Application.ApiResponse.Powens;

public class PowensFluxApiResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("value")]
    public decimal Valeur { get; set; }
    
    [JsonPropertyName("wording")]
    public string? Libelle { get; set; }
}