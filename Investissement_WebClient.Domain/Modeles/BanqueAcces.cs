namespace Investissement_WebClient.Domain.Modeles;

public class BanqueAcces
{
    public int Id { get; set; }
    
    public required string AccesToken { get; set; }

    public int IdCompteCourant { get; set; } 
    
    public DateTime DateCreation { get; set; }
    
    public DateTime DateExpiration {  get; set; }
}