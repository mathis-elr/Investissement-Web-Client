using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IServiceActif
{
    Task<IEnumerable<(int Id,string Nom)>> GetNomActifsEnregistres();
}