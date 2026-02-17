namespace Investissement_WebClient.Core.Modeles.DTO;

public class InvestissementDto
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public string? NomModele { get; set; }
    
    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
}