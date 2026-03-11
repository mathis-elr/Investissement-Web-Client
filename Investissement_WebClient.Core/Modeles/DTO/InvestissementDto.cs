namespace Investissement_WebClient.Core.Modeles.DTO;

public class InvestissementDto
{  
    public DateTime Date { get; set; }
    
    public int? idModele { get; set; }

    public string? Note { get; set; }

    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
}