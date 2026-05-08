using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Services.Patrimoine;

public interface IPatrimoineService
{
    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal);

    Task<VariationsDto> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal);

    Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieresPlusOuMoinsValues();

    //Task<IEnumerable<ProportionActif>> GetProportionParActifInvestit(decimal valeurPatrimoineCourant);

    //Task<IEnumerable<ProportionTypeActif>> GetProportionParTypeActifInvestit(decimal valeurPatrimoineCourant);

    Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal();

    Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart);
}