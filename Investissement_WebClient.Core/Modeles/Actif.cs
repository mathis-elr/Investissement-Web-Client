using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Core.Modeles;

public class Actif
{
    public int Id { get; set; }
    
    public string Nom { get; set; } 
    
    public ActifType Type { get; set; } 
    
    [StringLength(12, MinimumLength = 12, ErrorMessage = "L'ISIN doit faire exactement 12 caractères.")]
    [RegularExpression(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]$", ErrorMessage = "Format ISIN invalide.")]
    public string? Isin { get; set; } 
    
    public string Symbole { get; set; }
    
    public ActifRisque Risque { get; set; }
}