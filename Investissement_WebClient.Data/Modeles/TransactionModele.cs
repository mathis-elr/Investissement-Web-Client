namespace Investissement_WebClient.Data.Modeles
{
    public class TransactionModele
    {
        /*ATTRIBUTS*/
        public long id { get; set; }
        public string actif { get; set; }
        public long? quantite { get; set; }
        public long id_modele { get; set; }


        /*CONSTRUCTEUR*/
        public TransactionModele(long id, string actif, long? quantite, long idModele)
        {
            // Utiliser 'this.' pour pointer clairement vers l'attribut de la classe
            this.id = id;
            this.actif = actif;
            this.quantite = quantite;
            this.id_modele = idModele; // Attribution corrigée
        }

        public TransactionModele() { } // Ajouté pour l'initialiseur d'objet
    }
}
