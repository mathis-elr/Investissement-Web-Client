using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IModeleService
{
    Task<IEnumerable<ItemDto>> GetModeles();
    
    Task<List<TransactionDto>> GetCompositionModele(int idModele);
        
    Task AjouterModele(string nomModele, List<TransactionDto> compositionModele);
    
    Task UpdateModele(ItemDto item, List<TransactionDto> compositionModele);
    
    Task DeleteModeles(List<int> modeles);
}