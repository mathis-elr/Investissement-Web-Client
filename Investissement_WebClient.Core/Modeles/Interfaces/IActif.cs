namespace Investissement_WebClient.Core.Modeles;

public interface IActif
{
    public int Id { get; set; }
    
    public string Nom { get; set; } 
    
    public ActifType? Type { get; set; } 

    public string? Isin { get; set; } 
    
    public string Symbole { get; set; }
    
    public ActifRisque? Risque { get; set; }
}