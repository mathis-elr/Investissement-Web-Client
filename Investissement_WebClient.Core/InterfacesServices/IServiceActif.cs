using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IServiceActif
{
    Task<IEnumerable<(int Id,string Nom)>> GetActifsEnregistres();
    
    Task<IEnumerable<(int Id,string Nom)>> GetActifsDisponibles();
    
    Task<Actif> GetActifDisponible(int idActif);
    
    Task<ActifEnregistre> GetActifEnregistre(int idActif);
    
    Task SupprimerActifs(List<int> idActifs);
    
    Task AjouterActif(Actif actif);
    
    Task ModifierActif(ActifEnregistre actif);
}