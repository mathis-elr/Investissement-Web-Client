namespace Investissement_WebClient.Core.Modeles.DTO;

public record PreparationTransaction
{
    public int IdActif { get; set; }
    public string NomActif { get; set; } = string.Empty;
    public double? Quantite { get; set; }
    public double? Prix { get; set; }
}