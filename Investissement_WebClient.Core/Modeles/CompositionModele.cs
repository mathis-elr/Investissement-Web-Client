namespace Investissement_WebClient.Core.Modeles;

public class CompositionModele
{
    public ActifEnregistre ActifEnregistre { get; set; } = null!;
    public int IdActifEnregistre { get; set; }
    
    public Modele Modele { get; set; } = null!;
    public int IdModele { get; set; }
    
    public double? Quantite { get; set; }
}