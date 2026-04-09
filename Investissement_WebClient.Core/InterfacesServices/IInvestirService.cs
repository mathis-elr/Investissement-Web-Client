using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IInvestirService
{
   
    Task<IEnumerable<InvestissementGetDto>> GetInvestissements();
    
    Task SaveInvestissement(InvestissementDto investissementDto);

    Task DeleteDernierInvest(InvestissementGetDto investissementGetDto);

    Task<IEnumerable<InvestissementParMois>> GetInvestissementParMois(double investissementMoyen);
}