using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services.Investissement
{
    public interface IInvestissementService
    {
        Task<IEnumerable<TransactionDto>> GetTransactions();

        Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(decimal investissementMoyenMensuel);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif);
    
        Task<decimal> CalculerValeurInvestissementTotal();

        Task<decimal> CalculerInvestissementMoyenMensuel();

        Task<IEnumerable<InfoInvestParActifDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif);

        Task AddTransactionsRange(IEnumerable<Transaction> transactions);
        
        Task AddFluxTradeRepublicRange(IEnumerable<FluxTradeRepublic> fluxBancaires);
    }
}
