using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IPositionInvestissementRepository
    {
        Task<List<PositionInvestissement>> GetByCompteBanqueId(int compteId);

        Task AddRange(List<PositionInvestissement> positions);

        Task SaveChanges();
    }
}
