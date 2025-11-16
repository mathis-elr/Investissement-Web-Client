namespace Investissement_WebClient.Data.Modeles
{
    public class TransactionModele
    {
        /*ATTRIBUTS*/
        public string actif { get; set; }
        public long quantite { get; set; }
        public long? id_modele { get; set; }


        /*CONSTRUCTEUR*/
        public TransactionModele(string actif, long quantite, long? idModele)
        {
            this.actif = actif;
            this.quantite = quantite;
            id_modele = idModele;
        }
    }
}
