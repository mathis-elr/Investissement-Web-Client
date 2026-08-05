using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IValeurPatrimoineRepository
    {
        Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValuesByUserId(int userId);

        Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(int userId);

        Task<DateTime?> GetDateDernierEnregistrement();

        Task<List<ValeurPatrimoine>> GetHistoriqueAnneeByUserId(int userId);

        Task<List<PositionPatrimoineUtilisateurDto>> GetPositionsPatrimoineParUtilisateur();

        Task AddRange(List<ValeurPatrimoine> valeursPatrimoine);
    }
}