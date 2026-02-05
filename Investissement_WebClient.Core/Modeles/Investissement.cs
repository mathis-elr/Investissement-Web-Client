namespace Investissement_WebClient.Core.Modeles;

public class Investissement
{
    public int Id { get; set; }
    
    public DateTime dateInvest { get; set; }
    
    public Modele? Modele { get; set; }
    public int IdModele { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; }
}