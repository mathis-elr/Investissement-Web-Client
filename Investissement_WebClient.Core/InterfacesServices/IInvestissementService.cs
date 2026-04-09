using Investissement_WebClient.Core.Modeles;
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
    }
}
