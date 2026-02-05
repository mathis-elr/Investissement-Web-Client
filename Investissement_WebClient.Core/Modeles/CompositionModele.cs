namespace Investissement_WebClient.Core.Modeles;

public class CompositionModele
{
    public ActifEnregistre ActifEnregistre { get; set; }
    public int IdActifEnregistre { get; set; }
    
    public Modele Modele { get; set; }
    public int IdModele { get; set; }
    
    public double? Quantite { get; set; }
}