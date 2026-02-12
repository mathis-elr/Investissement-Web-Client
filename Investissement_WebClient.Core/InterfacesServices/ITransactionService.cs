using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface ITransactionService
{
    Task<IEnumerable<DetailsActifDto>> GetDetailsActif();
    
    Task SaveInvestissement(int? idModele, DateTime dateInvest,List<TransactionDto> transactionsInvestissement);
}