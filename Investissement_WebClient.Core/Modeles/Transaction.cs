namespace Investissement_WebClient.Core.Modeles;

public class Transaction
{
    public int Id { get; set; }
    
    public decimal Quantite { get; set; }
    
    public decimal Prix { get; set; }
    
    public decimal? Frais { get; set; }

    public Actif Actif { get; set; } = null!;
    public int IdActifEnregistre { get; set; }
    
    public Investissement Investissement { get; set; } = null!;
    public int IdInvestissement { get; set; }
}