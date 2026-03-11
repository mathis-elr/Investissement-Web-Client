using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IPatrimoineService
{
    Task<decimal> CalculerValeurPatrimoineCourante();
    
    Task<decimal> CalculerValeurInvestissementTotal();

    Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal);

    Task<VariationsDto> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal);

    Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieres();

    Task<IEnumerable<ProportionActif>> GetProportionParActifInvestit(decimal valeurPatrimoineCourant);

    Task<IEnumerable<ProportionTypeActif>> GetProportionParTypeActifInvestit(decimal valeurPatrimoineCourant);

    Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart);
}