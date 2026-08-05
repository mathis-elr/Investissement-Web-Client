using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ICategorieFluxRepository
    {
        Task<List<CategorieFlux>> GetAll();
    }
}
