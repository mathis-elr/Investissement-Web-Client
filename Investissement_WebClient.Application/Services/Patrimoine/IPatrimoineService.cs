using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Services.Patrimoine;

public interface IPatrimoineService
{
    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal);

    Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal);

    Task<IEnumerable<BougieJournaliereVM>> GetBougiesJournalieresPlusOuMoinsValues();

    //Task<IEnumerable<ProportionActif>> GetProportionParActifInvestit(decimal valeurPatrimoineCourant);

    //Task<IEnumerable<ProportionTypeActif>> GetProportionParTypeActifInvestit(decimal valeurPatrimoineCourant);

    Task<IEnumerable<BougieJournaliereVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal();

    Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart);
}