using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IActifEnregistreSQLite
    {
        public List<ActifModele> ReadActifs();
        public List<ActifEnresgistreModele> ReadActifsEnregistre();

        public void ajouterActif(ActifEnresgistreModele actifEnresgistre);

        public void modifierActif(ActifEnresgistreModele actifEnresgistre);

        public void supprActif(string nom);
    }
}
