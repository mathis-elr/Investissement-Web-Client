using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IActifService
{
    Task<IEnumerable<ActifDto>> GetActifsEnregistres();
    
    Task<IEnumerable<ActifDto>> GetActifsDisponibles();
    
    ActifTypesDto GetActifsParType(IEnumerable<ActifDto> actifs);
    
    Task SupprimerActifs(List<int> idActifs);
    
    Task AjouterActif(ActifDto actif);
    
    Task ModifierActif(ActifDto actif);
}