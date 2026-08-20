using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IBanqueRepository
    {
        Task<Banque?> GetByUserId(int userId);

        Task<IEnumerable<Banque>> GetAllByUserId(int userId);

        Task Add(Banque acces);

        Task Update(Banque acces);
    }
}
