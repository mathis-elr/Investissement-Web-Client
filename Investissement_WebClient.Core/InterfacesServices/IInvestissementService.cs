using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.Graphiques;
using Investissement_WebClient.Core.Modeles.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Core.InterfacesServices
{
    public interface IInvestissementService
    {
        Task<IEnumerable<TransactionVM>> GetTransactions();

        Task<IEnumerable<InvestissementParMois>> GetInvestissementParMois(decimal investissementMoyenMensuel);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif);
    
        Task<decimal> CalculerValeurInvestissementTotal();

        Task<decimal> CalculerInvestissementMoyenMensuel();

        Task<IEnumerable<InfoInvestParActif>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif);

        Task AddTransactionsRange(IEnumerable<Transaction> transactions);
        
        Task AddFluxTradeRepublicRange(IEnumerable<FluxTradeRepublic> fluxBancaires);
    }
}
