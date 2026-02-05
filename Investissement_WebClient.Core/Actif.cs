using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Core
{
    public class Actif
    {
        private readonly IActifEnregistreSQLite _IActif;
        public Actif(IActifEnregistreSQLite IActif) 
        { 
            _IActif = IActif;
        }

        public List<string> GetModes()
        {
            return ["Ajouter", "Modifier", "Supprimer"];
        }
        
        public List<ActifModele> GetListeActifs()
        {
            return _IActif.ReadActifs();
        }

        public List<ActifEnresgistreModele> GetListeActifsEnresgistre()
        {
            return _IActif.ReadActifsEnregistre();
        }

        public List<string> GetListeNvRisque()
        {
            return ["Faible", "Moyen", "Fort"];
        }

        public void AjouterActif(ActifEnresgistreModele actifEnresgistre)
        {
            _IActif.ajouterActif(actifEnresgistre);
        }

        public void ModifierActif(ActifEnresgistreModele actifEnresgistre)
        {
            _IActif.modifierActif(actifEnresgistre);
        }

        public void SupprimerActif(string nom)
        {
            _IActif.supprActif(nom);
        }
    }
}
