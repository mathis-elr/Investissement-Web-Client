using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Core
{
    public class Actif
    {
        private readonly IActifSQLite _IActif;
        public Actif(IActifSQLite IActif) 
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

        public List<string> GetListeNvRisque()
        {
            return ["Faible", "Moyen", "Fort"];
        }

        public void AjouterActif(ActifModele actif)
        {
            _IActif.ajouterActif(actif);
        }

        public void ModifierActif(ActifModele actif)
        {
            _IActif.modifierActif(actif);
        }

        public void SupprimerActif(string nom)
        {
            _IActif.supprActif(nom);
        }
    }
}
