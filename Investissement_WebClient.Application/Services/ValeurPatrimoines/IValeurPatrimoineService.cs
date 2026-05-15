using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.Services.ValeurPatrimoines;

public interface IValeurPatrimoineService
{
    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal, int userId);

    Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId);

    Task<DateTime?> GetDateDernierEnregistrement(int userId);

    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValues(int userId);

    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId);
}