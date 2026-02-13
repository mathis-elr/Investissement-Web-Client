namespace Investissement_WebClient.Core.Modeles.DTO;

public class TransactionDto
{
    public int IdActif { get; set; }
    
    public string NomActif { get; set; } = string.Empty;
    
    public double? Quantite { get; set; }
    
    public double? Prix { get; set; }
    
    public double? Frais { get; set; }
}