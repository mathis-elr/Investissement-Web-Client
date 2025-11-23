namespace Investissement_WebClient.Data.Modeles
{
    public class TransactionModele
    {
        /*ATTRIBUTS*/
        public string? modele {  get; set; }
        public string actif { get; set; }
        public double? quantite { get; set; }


        /*CONSTRUCTEUR*/
        public TransactionModele(string? modele, string actif, double? quantite)
        {
            this.modele = modele;
            this.actif = actif;
            this.quantite = quantite;
        }

        public TransactionModele() { } // Ajouté pour l'initialiseur d'objet
    }
}
