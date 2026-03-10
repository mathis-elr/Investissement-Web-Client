namespace Investissement_WebClient.Core.Modeles.Graphiques;

public class BougieJournaliere
{
    public DateTime Date { get; set; }
    
    public decimal Ouverture { get; set; }
    
    public decimal Fermeture { get; set; }
    
    public decimal Haut { get; set; }
    
    public decimal Bas { get; set; }
}