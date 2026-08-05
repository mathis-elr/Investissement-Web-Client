using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IActifRepository
    {
        Task<List<Actif>> GetAll();

        Task<IEnumerable<string>> GetAllTickers();

        Task<int> Add(Actif actif);
    }
}
