using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Core.InterfacesServices
{
    public interface ITrTransactionsService
    {
        Task<string> GetSms();
    }
}
