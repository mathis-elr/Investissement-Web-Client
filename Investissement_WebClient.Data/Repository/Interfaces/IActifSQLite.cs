using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IActifSQLite
    {
        public List<ActifModele> ReadActifs();

        public void ajouterActif(ActifModele actif);

        public void modifierActif(ActifModele actif);

        public void supprActif(string nom);
    }
}
