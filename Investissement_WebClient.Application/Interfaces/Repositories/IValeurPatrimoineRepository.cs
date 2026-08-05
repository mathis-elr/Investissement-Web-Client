using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IValeurPatrimoineRepository
    {
        Task<IEnumerable<BougieJournaliereCandleChartDto>> GetBougiesJournalieresPlusOuMoinsValuesByUserId(int userId);

        Task<IEnumerable<BougieJournaliereCandleChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(int userId);

        Task<DateTime?> GetDateDernierEnregistrement();

        Task<List<ValeurPatrimoine>> GetAllHistoriqueByUserId(int userId);

        Task<List<PositionPatrimoineUtilisateurDto>> GetPositionsPatrimoineParUtilisateur();

        Task AddRange(List<ValeurPatrimoine> valeursPatrimoine);
    }
}