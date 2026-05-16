using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.Services.ValeurPatrimoines;

public interface IValeurPatrimoineService
{
    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValues(int userId);

    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId);

    Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId);

    Task<DateTime?> GetDateDernierEnregistrement();

    Task SaveValeurPatrimoine(Dictionary<string, decimal> prixParActif);
}