using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.InterfacesRepositories
{
    public interface IFluxInvestissementRepository
    {
        Task<IEnumerable<FluxInvestissementDto>> GetAllByUserId(int userId);

        Task<FluxInvestissement?> GetLastByUserId(int userId);

        Task<List<PositionActifDto>> GetPositionsParActifByUserId(int userId);

        Task<decimal> GetValeurInvestissementTotalByUserId(int userId);

        Task<IEnumerable<PositionInvestissementDto>> GetPositionsInvestiesParActifByUserId(int userId);

        Task<List<InvestissementParMoisVM>> GetInvestissementParMoisByUserId(int userId);

        Task AddRange(List<FluxInvestissement> flux);
    }
}
