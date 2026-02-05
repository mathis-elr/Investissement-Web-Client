using Investissement_WebClient.Core;
using System.Diagnostics;
using Investissement_WebClient.Core.Modeles;
using Transaction = System.Transactions.Transaction;


namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ModeleViewModel
    {
        // private ModeleInvest modeleInvest;

        public List<string> ListeModes { get; set; }
        public string selectedMode { get; set; }
        public string? selectedNomModele { get; set; } = null;
        public List<string> ListeNomsActif { get; set; }
        //public List<string> ListeActifModele { get; set; }
        public List<CompositionModele> ListeTransactionsModele { get; set; } = new List<CompositionModele>();
        public double? selectedQuantite { get; set; } = null;

        private string selectedModeleEdit { get; set; }
        public List<string> ListeModeles { get; set; }
        public List<CompositionModele> ListeTransactionsModeleEdit { get; set; } = new List<CompositionModele>();

        public List<string> ListeModelesAsuppr {  get; set; } = new List<string>();

        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = string.Empty;

        public string SelectedModeleEdit
        {
            get { return selectedModeleEdit; }
            set
            {
                selectedModeleEdit = value;
                LoadTransactionsModele(selectedModeleEdit);
            }
        }

        private void LoadModes()
        {
            // ListeModes = modeleInvest.GetModes();
        }

        private void LoadNomsActif()
        {
            // ListeNomsActif = modeleInvest.GetNomsActif();
        }

        private void LoadModeles()
        {
            // ListeModeles = modeleInvest.GetModeles();
            selectedMode = ListeModes.First();
        }

        private void LoadTransactionsModele(string modele)
        {
            // ListeTransactionsModeleEdit = modeleInvest.GetTransactionsModele(modele);
        }

        public ModeleViewModel()
        {

            LoadModes();

            LoadNomsActif();
            LoadModeles();

            selectedModeleEdit = "Aucun";
        }
        
        public void ChangeMode()
        {
            selectedMode = selectedMode == "Ajouter" ? "Modifier" : "Ajouter";
        }

        public void AddActifModele()
        {
            // ListeTransactionsModele.Add(new TransactionModele(selectedNomModele, ListeNomsActif.First(), null));
        }

        public void DellActifModele()
        {
            // ListeTransactionsModele.Remove(transaction);
        }

        public void Ajouter()
        {
            hasError = false;
            errorMessage = string.Empty;

            if(selectedNomModele == null)
            {
                hasError = true;
                errorMessage = "Entrez un nom pour votre modèle";
                return;
            }

            if(ListeModeles.Contains(selectedNomModele))
            {
                hasError = true;
                errorMessage = "Ce nom de modele existe déjà";
                return;
            }

            // foreach (TransactionModele transaction in ListeTransactionsModele)
            // {
                // transaction.modele = selectedNomModele;
            // }

            // modeleInvest.AjouterModele(ListeTransactionsModele);

            // ListeTransactionsModele = [];
            selectedNomModele = null;
            LoadModeles();
        }

        public void AddActifModeleEdit()
        {
            // ListeTransactionsModeleEdit.Add(new TransactionModele(selectedModeleEdit, ListeNomsActif.First(), null));
        }

        public void DellActifModeleEdit()
        {
            // ListeTransactionsModeleEdit.Remove(transaction);
        }

        public void Editer()
        {
            hasError = false;
            errorMessage = string.Empty;

            // modeleInvest.EditerModele(ListeTransactionsModeleEdit);

            // ListeTransactionsModeleEdit = [];
            selectedModeleEdit = "Aucun";
            LoadModeles();
        }

        public void ChangerEtatSuppression(string modele)
        {
            if (ListeModelesAsuppr.Contains(modele))
            {
                ListeModelesAsuppr.Remove(modele);
            }
            else
            {
                ListeModelesAsuppr.Add(modele);
            }
        }

        public void Supprimer()
        {
            hasError = false;
            errorMessage = string.Empty;

            foreach(string modele in ListeModelesAsuppr)
            {
                // modeleInvest.SupprimerModele(modele);
            }

            LoadModeles();
        }
    }
}
