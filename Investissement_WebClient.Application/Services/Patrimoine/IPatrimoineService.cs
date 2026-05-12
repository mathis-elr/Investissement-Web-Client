using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Services.Patrimoine;

public interface IPatrimoineService
{
    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal);

    Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal);

    Task<DateTime?> GetDateDernierEnregistrement();

    Task<IEnumerable<BougieJournaliereVM>> GetBougiesJournalieresPlusOuMoinsValues();

    Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif);


    Task<IEnumerable<BougieJournaliereVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal();

    Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart);
}