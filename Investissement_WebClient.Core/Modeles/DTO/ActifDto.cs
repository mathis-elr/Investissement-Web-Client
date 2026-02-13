namespace Investissement_WebClient.Core.Modeles.DTO;

public class ActifDto
{
    public int Id { get; set; }
    
    public string Nom { get; set; } = null!;
    
    public ActifType Type { get; set; } 
    
    public string? Isin { get; set; } 
    
    public string Symbole { get; set; } = null!;
    
    public ActifRisque Risque { get; set; }
}