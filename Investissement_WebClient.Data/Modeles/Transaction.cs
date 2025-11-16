using System;


namespace Investissement_WebClient.Data.Modeles
{
    public class Transaction
    {        
        /*ATTRIBUTS*/
        public long id { get; set; }
        public DateTime date { get; set; }
        public string actif { get; set; }
        public double quantite { get; set; }
        public double prix { get; set; }



        /*CONSTRUCTEUR*/
        public Transaction(DateTime date, string actif, double quantite, double prix)
        {
            this.date = date;
            this.actif = actif;
            this.quantite = quantite;
            this.prix = prix;
        }

        /*ENCAPUSULATION*/
        //aucune pour le moment
    }
}
