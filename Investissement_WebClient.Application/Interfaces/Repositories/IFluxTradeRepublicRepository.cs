using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IFluxTradeRepublicRepository
    {
        Task<IEnumerable<FluxTradeRepublicDto>> GetAllByUserId(int userId);

        Task<FluxTradeRepublic?> GetLastByUserId(int userId);

        Task<FluxTradeRepublic?> GetFirstDateByUserId(int userId);

        Task<List<PositionActifDto>> GetPositionsParActifByUserId(int userId);

        Task<decimal> GetValeurInvestissementTotalByUserId(int userId);

        Task<IEnumerable<PositionInvestissementDto>> GetPositionsInvestiesParActifByUserId(int userId);

        Task<List<InvestissementParMoisDto>> GetInvestissementParMoisByUserId(PeriodeHistoriqueInvest periode, int userId);

        Task AddRange(List<FluxTradeRepublic> flux);
    }
}
