using System;

namespace Investissement_WebClient.Data.Modeles
{
    public class ValeurPatrimoine
    {
        DateTime date { get; set; }
        double valeurEUR { get; set; }

        public ValeurPatrimoine(DateTime date, double valeurEUR)
        {
            this.date = date;
            this.valeurEUR = valeurEUR;
        }
    }
}
