namespace Investissement_WebClient.Core.Modeles;

public class Modele
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
    
    public ICollection<Investissement>? Investissements { get; set; }
    
    public ICollection<CompositionModele>? Composition { get; set; }
}