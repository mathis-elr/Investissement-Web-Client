namespace Investissement_WebClient.Core.Modeles;

public class Modele
{
    public int Id { get; set; }
    
    public string Nom { get; set; }
    
    public ICollection<ActifEnregistre> ActifEnregistres { get; set; }
    
    public ICollection<Investissement>? Investissements { get; set; }
    
    public ICollection<CompositionModele>? CompositionModeles { get; set; }
}