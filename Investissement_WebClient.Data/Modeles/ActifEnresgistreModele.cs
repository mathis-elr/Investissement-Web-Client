namespace Investissement_WebClient.Data.Modeles
{
    public class ActifEnresgistreModele
    {
        /*ATTRIBUTS*/
        public string nom { get; set; }
        public string symbole { get; set; }
        public string type { get; set; }
        public string? isin { get; set; }
        public string risque { get; set; }

        /*CONSTRUCTEUR*/
        public ActifEnresgistreModele(string nom, string symbole, string type, string? isin, string risque)
        {
            this.nom = nom;
            this.symbole = symbole;
            this.type = type;
            this.isin = isin;
            this.risque = risque;
        }
    }
}
