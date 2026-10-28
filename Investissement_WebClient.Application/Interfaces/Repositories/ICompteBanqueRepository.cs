using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ICompteBanqueRepository
    {
        Task<IEnumerable<CompteBanque>> GetAllNonInvestissement();

        Task<IEnumerable<CompteBanque>> GetAllInvestissement();

        Task<IEnumerable<CompteBanque>> GetAllByBanqueId(int banqueId);

        Task<List<CompteBanque>> GetAllByUserId(int userId);

        Task<CompteBanque?> GetById(int id);

        Task UpdateNom(SourceDto source);

        Task Add(CompteBanque compte);

        Task Update(CompteBanque compte);

        Task SaveChanges();
    }
}
