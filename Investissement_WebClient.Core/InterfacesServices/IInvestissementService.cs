using Investissement_WebClient.Core.Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.Core.InterfacesServices
{
    public interface IInvestissementService
    {
        Task<IEnumerable<TransactionVM>> GetTransactions();

        Task<IEnumerable<InvestissementParMois>> GetInvestissementParMois(decimal investissementMoyenMensuel);
        
        Task<decimal> CalculerValeurCourante();
    
        Task<decimal> CalculerValeurInvestissementTotal();

        Task<decimal> CalculerInvestissementMoyenMensuel();

        Task AddTransactionsRange(IEnumerable<Transaction> transactions);
        
        Task AddFluxBancairesRange(IEnumerable<FluxBancaire> fluxBancaires);
    }
}
