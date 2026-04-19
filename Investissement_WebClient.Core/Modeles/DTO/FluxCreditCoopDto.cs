using System.Text.Json.Serialization;

namespace Investissement_WebClient.Core.Modeles.DTO;

public class FluxCreditCoopDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("value")]
    public decimal Valeur { get; set; }
    
    [JsonPropertyName("wording")]
    public string LibelleRecu { get; set; }
}