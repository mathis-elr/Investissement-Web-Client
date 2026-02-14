using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IModeleService
{
    Task<List<ModeleDto>> GetModeles();
    
    Task<IEnumerable<ModeleCompositionDto>> GetModelesComposition();
    
    Task<List<TransactionDto>> GetCompositionModele(int idModele);
        
    Task AjouterModele(string nomModele, List<TransactionDto> compositionModele);
    
    Task UpdateModele(ModeleDto modele, List<TransactionDto> compositionModele);
    
    Task DeleteModeles(List<int> modeles);
}