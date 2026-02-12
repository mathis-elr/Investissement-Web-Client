using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IServiceActif
{
    Task<IEnumerable<ItemDto>> GetActifsEnregistres();
    
    Task<IEnumerable<ItemDto>> GetActifsDisponibles();
    
    Task<Actif> GetActifDisponible(int idActif);
    
    Task<ActifEnregistre> GetActifEnregistre(int idActif);
    
    Task SupprimerActifs(List<int> idActifs);
    
    Task AjouterActif(Actif actif);
    
    Task ModifierActif(ActifEnregistre actif);
}