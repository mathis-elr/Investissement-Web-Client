using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IValeurPatrimoineService
    {
        Task<IEnumerable<BougieCandleChartDto>> GetBougiesPlusValueByUserId(LapsTemps periode, Granulometrie granulometrie, int userId);

        Task<IEnumerable<BougieCandleChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId);

        Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId);

        Task<DateTime?> GetDateDernierEnregistrement();

        Task SaveValeurPatrimoine(Dictionary<string, decimal> prixParActif);
    }
}

