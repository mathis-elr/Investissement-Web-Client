using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.Services.ValeurPatrimoines;

public interface IValeurPatrimoineService
{
    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal);

    Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal);

    Task<DateTime?> GetDateDernierEnregistrement();

    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValues();

    Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif);


    Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal();

    Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart);
}