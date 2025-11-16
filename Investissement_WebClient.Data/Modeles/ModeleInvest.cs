namespace Investissement_WebClient.Data.Modeles
{
    public class ModeleInvest
    {
        /*ATTRIBUTS*/
        public long id { get; set; }
        public string nom { get; set; }
        public string description { get; set; }


        /*CONSTRUCTEUR*/
        public ModeleInvest( string nom, string description)
        {
            this.nom = nom;
            this.description = description;
            //l'id est recuperé une fois l'actif ajouté pour avoir le meme que celui de la bdd
        }
    }
}
