using System;

namespace Investissement_WebClient.Data.Modeles
{
    public class InvestissementTotal
    {
        DateTime date { get; set; }
        double quantiteEUR { get; set; }

        public InvestissementTotal(DateTime date, double quantiteEUR)
        {
            this.date = date;
            this.quantiteEUR = quantiteEUR;
        }
    }
}
