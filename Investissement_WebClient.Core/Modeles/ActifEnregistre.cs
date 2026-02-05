namespace Investissement_WebClient.Core.Modeles;

public class ActifEnregistre
{
    public int Id { get; set; }
    
    public string Nom { get; set; }
    
    public string Symbole { get; set; }
    
    public ActifType Type { get; set; }
    
    public string? Isin { get; set; }
    
    public ActifRisque Risque { get; set; }
    
    public ICollection<Modele>? Modeles { get; set; }
    
    public ICollection<Transaction>? Transactions { get; set; }
    
    public ICollection<CompositionModele>? CompositionModeles { get; set; }
}