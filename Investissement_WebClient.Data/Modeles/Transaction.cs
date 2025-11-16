namespace Investissement_WebClient.Data.Modeles
{
    public class Transaction
    {        
        /*ATTRIBUTS*/
        public DateTime date { get; set; }
        public string actif { get; set; }
        public double? quantite { get; set; }
        public double? prix { get; set; }



        /*CONSTRUCTEURS*/
        public Transaction(DateTime date, string actif, double? quantite, double? prix)
        {
            this.date = date;
            this.actif = actif;
            this.quantite = quantite;
            this.prix = prix;
        } 

        public Transaction(DateTime date, string actif) 
        { 
            this.date = date;   
            this.actif = actif;
        }
    }
}
