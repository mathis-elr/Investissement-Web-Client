using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Core.Modeles
{
    public class TransactionVM
    {
        public DateTimeOffset Date { get; set; }

        public string? Actif { get; set; }
        
        public string? Ticker { get; set; }

        public decimal? Prix { get; set; }

        public decimal? Quantite { get; set; }
    }
}
