namespace Investissement_WebClient.Core.Modeles;

public class FluxBancaire
{
    public string? Id { get; init; }
    
    public DateTimeOffset? Date { get; init; }
    
    public string? Type { get; init; }
    
    public string? Expediteur { get; init; }
    
    public decimal? Montant { get; init; }
}