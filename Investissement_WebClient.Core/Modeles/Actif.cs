namespace Investissement_WebClient.Core.Modeles;

public class Actif
{
    public int Id { get; set; }
    
    public string Name { get; set; } 
    
    public string Type { get; set; } 
    
    public string? Isin { get; set; } 
    
    public string Symbole { get; set; }
}