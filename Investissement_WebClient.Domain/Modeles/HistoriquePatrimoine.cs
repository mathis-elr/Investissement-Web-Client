namespace Investissement_WebClient.Domain.Modeles;

public class HistoriquePatrimoine
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal InvestissementTotal { get; set; }
    
    public decimal Valeur { get; set; }
}