namespace Investissement_WebClient.Core.Modeles.DTO;

public class ActifTypesDto
{
    public IEnumerable<ActifDto> Etfs { get; set; } = [];
    
    public IEnumerable<ActifDto> Etcs { get; set; } = [];
    
    public IEnumerable<ActifDto> Actions { get; set; } = [];
    
    public IEnumerable<ActifDto> Cryptos { get; set; } = [];

    public IEnumerable<ActifDto> Obligations { get; set; } = [];
}