namespace Investissement_WebClient.Core.Modeles.DTO;

public class ModeleCompositionDto
{
    public  int Id { get; set; }
    
    public string Nom { get; set; } = string.Empty;

    public IEnumerable<TransactionDto> Composition { get; set; } = [];
}