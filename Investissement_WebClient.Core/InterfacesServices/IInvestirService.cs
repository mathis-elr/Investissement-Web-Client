using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IInvestirService
{
    Task<IEnumerable<DetailsActifDto>> GetDetailsActif();
    
    Task<IEnumerable<InvestissementDto>> GetInvestissements();
    
    Task SaveInvestissement(int? idModele, DateTime dateInvest,List<TransactionDto> transactionsInvestissement);
}