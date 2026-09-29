using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IBanqueRepository
    {
        Task<IEnumerable<Banque>> GetAll();

        Task<Banque?> GetByUserId(int userId);

        Task<Banque?> GetByCompteId(int compteId);

        Task<IEnumerable<Banque>> GetAllByUserId(int userId);

        Task Add(Banque acces);

        Task Update(Banque acces);
    }
}
