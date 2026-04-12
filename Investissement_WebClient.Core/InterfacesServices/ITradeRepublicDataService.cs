using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Investissement_WebClient.Core.InterfacesServices
{
    public interface ITradeRepublicDataService
    {
        Task<(string message, int codeHTTP)> GetSms();

        Task<(string message, int codeHTTP)> ConfirmSms(string codeSms);

        Task<int> ChargerTransactions();
    }
}
