using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IBanqueAccesRepository
    {
        Task<BanqueAcces?> GetByUserId(int userId);

        Task<IEnumerable<int>> GetAll();

        Task Add(BanqueAcces acces);
    }
}
