using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IPatrimoineService
{
    Task<double> CalculerValeurPatrimoineCourante();
    
    Task<double> CalculerValeurInvestissementTotal();

    Task SaveValeurPatrimoine(double valeurPatrimoine, double valeurInvestissementTotal);

    Task<VariationsDto> GetVariations(double valeurActuelle, double valeurInvestissementTotal);

    Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieres();

    Task<IEnumerable<ProportionActif>> GetProportionParActifInvestit(double valeurPatrimoineCourant);

    Task<IEnumerable<ProportionTypeActif>> GetProportionParTypeActifInvestit(double valeurPatrimoineCourant);
}