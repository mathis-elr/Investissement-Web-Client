using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ICompteBanqueRepository
    {
        Task<IEnumerable<CompteBanque?>> GetAll();

        Task<IEnumerable<CompteBanque?>> GetAllByBanqueId(int banqueId);

        Task<CompteBanque?> GetByBanqueId(int banqueId);

        Task Add(CompteBanque compte);

        Task Update(CompteBanque compte);
    }
}
