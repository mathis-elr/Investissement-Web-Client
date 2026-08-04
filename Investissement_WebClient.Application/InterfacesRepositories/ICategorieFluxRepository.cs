using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.InterfacesRepositories
{
    public interface ICategorieFluxRepository
    {
        Task<List<CategorieFlux>> GetAll();
    }
}
