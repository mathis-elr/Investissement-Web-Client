namespace Investissement_WebClient.Core.Modeles.DTO;

public class InvestissementGetDto
{
    public int Id { get; set; }

    public DateTime DateInvest { get; set; }

    public string? Note { get; set; }

    public ModeleDto? Modele { get; set; }

    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
}