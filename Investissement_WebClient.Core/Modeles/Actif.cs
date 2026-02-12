using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Core.Modeles;

public class Actif : IActif
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nom requis.")]
    public string Nom { get; set; } = null!;
    
    [Required(ErrorMessage = "Type requis.")]
    public ActifType? Type { get; set; } 
    
    [StringLength(12, MinimumLength = 12, ErrorMessage = "L'ISIN doit faire exactement 12 caractères.")]
    [RegularExpression(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]$", ErrorMessage = "Format de l'ISIN invalide.")]
    public string? Isin { get; set; } 
    
    [Required(ErrorMessage = "Symbole requis.")]
    public string Symbole { get; set; } = null!;
    
    [Required(ErrorMessage = "Niveau de risque requis.")]
    public ActifRisque? Risque { get; set; }
}