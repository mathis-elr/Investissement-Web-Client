namespace Investissement_WebClient.Data.Modeles
{
    public class ModeleInvest
    {
        /*ATTRIBUTS*/
        public long id { get; set; }
        public string nom { get; set; }


        /*CONSTRUCTEURS*/
        public ModeleInvest( string nom)
        {
            this.nom = nom;
            //l'id est recuperé une fois l'actif ajouté pour avoir le meme que celui de la bdd
        }

        public ModeleInvest(long id, string nom)
        {
            this.id = id;
            this.nom = nom;
        }
    }
}
