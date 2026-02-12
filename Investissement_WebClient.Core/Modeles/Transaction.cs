namespace Investissement_WebClient.Core.Modeles;

public class Transaction
{
    public int Id { get; set; }
    
    public double Quantite { get; set; }
    
    public double Prix { get; set; }
    
    public double? Frais { get; set; }

    public ActifEnregistre ActifEnregistre { get; set; } = null!;
    public int IdActifEnregistre { get; set; }
    
    public Investissement Investissement { get; set; } = null!;
    public int IdInvestissement { get; set; }
}