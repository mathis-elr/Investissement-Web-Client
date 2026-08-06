using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IValeurPatrimoineRepository
    {
        Task<IEnumerable<BougieCandleChartDto>> GetBougiesPlusValueByUserId(LapsTemps periode, Granulometrie granulometrie, int userId);

        Task<IEnumerable<BougieCandleChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(int userId);

        Task<DateTime?> GetDateDernierEnregistrement();

        Task<List<ValeurPatrimoine>> GetAllHistoriqueByUserId(int userId);

        Task<List<PositionPatrimoineUtilisateurDto>> GetPositionsPatrimoineParUtilisateur();

        Task AddRange(List<ValeurPatrimoine> valeursPatrimoine);
    }
}