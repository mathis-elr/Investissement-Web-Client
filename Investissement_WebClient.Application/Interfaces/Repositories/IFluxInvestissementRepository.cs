using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IFluxInvestissementRepository
    {
        Task<IEnumerable<FluxInvestissementDto>> GetAllByUserId(int userId);

        Task<FluxInvestissement?> GetLastByUserId(int userId);

        Task<List<PositionActifDto>> GetPositionsParActifByUserId(int userId);

        Task<decimal> GetValeurInvestissementTotalByUserId(int userId);

        Task<IEnumerable<PositionInvestissementDto>> GetPositionsInvestiesParActifByUserId(int userId);

        Task<List<InvestissementParMoisDto>> GetInvestissementParMoisByUserId(int userId);

        Task AddRange(List<FluxInvestissement> flux);
    }
}
