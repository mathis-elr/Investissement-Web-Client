using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IPatrimoineService
{
    Task<double> CalculerValeurPatrimoineCourante();
    
    Task<double> CalculerValeurInvestissementTotal();

    Task<VariationsDto> GetVariations(double valeurActuelle, double valeurInvestissementTotal);

    Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieres();
}