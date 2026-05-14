using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Domain.Modeles;

public class FluxInvestissement
{
    public string? Id { get; init; }

    public DateTime Date { get; init; }

    public TypeFlux Type { get; init; }

    public int ActifId { get; set; }
    public Actif? Actif {  get; set; }

    public decimal Prix { get; init; }

    public decimal Quantite { get; init; }
    
    public decimal? Frais { get; init; }

    public decimal Total { get; init; }
}