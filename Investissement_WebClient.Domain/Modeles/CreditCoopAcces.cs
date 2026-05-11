namespace Investissement_WebClient.Domain.Modeles;

public class CreditCoopAcces
{
    public int Id { get; set; }
    
    public string? AccesToken { get; set; }

    public int IdCompteCourant { get; set; } 
    
    public DateTime DateCreation { get; set; }
    
    public DateTime DateExpiration {  get; set; }
}