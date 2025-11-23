using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Core
{
    public class ModeleInvest
    {
        private readonly IModeleInvestSQLite _IModeleInvest;

        public ModeleInvest(IModeleInvestSQLite interfaceModeleInvest)
        {
            _IModeleInvest = interfaceModeleInvest;
        }

        public List<string> GetModes()
        {
            return ["Ajouter", "Modifier", "Mes modèles"];
        }

        public List<string> GetNomsActif()
        {
            return _IModeleInvest.ReadNomsActif();
        }

        public List<string> GetModeles()
        {
            return _IModeleInvest.ReadModeles();
        }

        public List<TransactionModele> GetTransactionsModele(string modele)
        {
            return _IModeleInvest.ReadTransactionsModele(modele);
        }

        public void AjouterModele(List<TransactionModele> transactionsModele)
        {
            _IModeleInvest.AjouterModele(transactionsModele);
        }

        public void EditerModele(List<TransactionModele> transactionsModele)
        {
            this.SupprimerModele(transactionsModele[0].modele);
            this.AjouterModele(transactionsModele);
        }

        public void SupprimerModele(string modele)
        {
            _IModeleInvest.SupprimerModele(modele);
        }
    }
}
