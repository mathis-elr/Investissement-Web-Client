using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Core.Modeles;

public class ActifEnregistre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nom requis.")]
    public string Nom { get; set; } = null!;
    
    [Required(ErrorMessage = "Symbole requis.")]

    public string Symbole { get; set; } = null!;
    
    [Required(ErrorMessage = "Type requis.")]

    public ActifType? Type { get; set; } = null!;
    
    [StringLength(12, MinimumLength = 12, ErrorMessage = "L'ISIN doit faire exactement 12 caractères.")]
    [RegularExpression(@"^[A-Z]{2}[A-Z0-9]{9}[0-9]$", ErrorMessage = "Format ISIN invalide.")]
    public string? Isin { get; set; }
    
    [Required(ErrorMessage = "Niveau de risque requis.")]
    public ActifRisque? Risque { get; set; }
    
    public ICollection<Transaction>? Transactions { get; set; }
    
    public ICollection<CompositionModele>? Composition { get; set; }
}