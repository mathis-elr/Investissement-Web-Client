using Investissement_WebClient.Application.DTO.Patrimoine;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IValeurPatrimoineService
    {
        Task<IEnumerable<BougieJournaliereCandleChartDto>> GetBougiesJournalieresPlusOuMoinsValues(int userId);

        Task<IEnumerable<BougieJournaliereCandleChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId);

        Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId);

        Task<DateTime?> GetDateDernierEnregistrement();

        Task SaveValeurPatrimoine(Dictionary<string, decimal> prixParActif);
    }
}

