namespace Investissement_WebClient.Core.Modeles.DTO;

public class TransactionDto
{
    public int IdActif { get; set; }
    
    public string NomActif { get; set; } = string.Empty;
    
    public decimal? Quantite { get; set; }
    
    public decimal? Prix { get; set; }
    
    public decimal? Frais { get; set; }
}