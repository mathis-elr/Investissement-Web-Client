namespace Investissement_WebClient.Core.Modeles;

public class CreditCoopAcces
{
    public int Id { get; set; }
    
    public string? AccesToken { get; set; }
    
    public DateTime DateCreation { get; set; }
    
    public DateTime DateExpiration => DateCreation.AddDays(90);
}