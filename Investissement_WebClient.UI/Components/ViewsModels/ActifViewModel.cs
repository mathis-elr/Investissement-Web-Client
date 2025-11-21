using Investissement_WebClient.Core;
using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Repository.SQLite;
using System.Diagnostics;


namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ActifViewModel
    {
        private readonly Actif actif;

        public string? selectedNom { get; set; } = null;
        public string? selectedSymbole { get; set; } = null;
        public string? selectedType { get; set; } = null;
        public string? selectedISIN { get; set; } = null;
        public string? selectedNvRisque { get; set; } = null;

        public List<string>? ListeNvRisque { get; set; }
        public List<ActifModele>? ListeActifs { get; set; }
        public List<string>? ListeNomsActifs { get; set; }

        private ActifModele? selectActifModelEdit { get; set; } = null;
        private string selectedActifEdit { get; set; }
        public string? selectedNomEdit { get; set; } = null;
        public string? selectedSymboleEdit { get; set; } = null;
        public string? selectedTypeEdit { get; set; } = null;
        public string? selectedISINEdit { get; set; } = null;
        public string? selectedNvRisqueEdit { get; set; } = null;

        public List<string> ListeActifASuppr { get; set; } = new List<string>();

        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = string.Empty;

        public string SelectedActifEdit
        {
            get {  return selectedActifEdit; }
            set
            {
                selectedActifEdit = value;
                if(value != "Aucun")
                {
                    selectActifModelEdit = ListeActifs.FirstOrDefault(actif => actif.nom == value);
                    selectedNomEdit = selectActifModelEdit.nom;
                    selectedSymboleEdit = selectActifModelEdit.symbole;
                    selectedTypeEdit = selectActifModelEdit.type;
                    selectedISINEdit = selectActifModelEdit.isin;
                    selectedNvRisqueEdit = selectActifModelEdit.risque;
                }
                else
                {
                    selectActifModelEdit = null;
                    selectedNomEdit = string.Empty;
                    selectedSymboleEdit = string.Empty;
                    selectedTypeEdit = string.Empty;
                    selectedISINEdit = null;
                    selectedNvRisqueEdit = string.Empty;
                }
            }
        }

        public void LoadNvRisque()
        {
            ListeNvRisque = actif.GetListeNvRisque();
        }
        public void LoadActifs()
        {
            ListeActifs = actif.GetListeActifs();
            ListeNomsActifs = ListeActifs.Select(nom => nom.nom).ToList();
        }

        public ActifViewModel() 
        {
            IActifSQLite Iactif = new ActifSQLite(BDDService.ConnectionString);
            actif = new Actif(Iactif);

            LoadNvRisque();
            LoadActifs();

            selectedActifEdit = "Aucun";
        }

        public void Ajouter()
        {
            hasError = false;
            errorMessage = string.Empty;

            if(selectedNom == null|| selectedSymbole == null || selectedType == null || selectedNvRisque == null)
            {
                hasError = true;
                errorMessage = "Le nom, le symbole, le type et le niveau de risque doivent être renseignés pour pouvoir ajouter un actif.";
                return;
            }

            ActifModele selectActifModelAdd = new ActifModele(selectedNom, selectedSymbole, selectedType, selectedISIN, selectedNvRisque);
            actif.AjouterActif(selectActifModelAdd);

            selectedNom = null;
            selectedSymbole = null;
            selectedType = null;
            selectedISIN = null;
            selectedNvRisque = "Niveau de risque";

            LoadActifs();
        }

        public void Editer()
        {
            hasError = false;
            errorMessage = string.Empty;

            if (selectedNomEdit == null || selectedSymboleEdit == null || selectedTypeEdit == null || selectedNvRisqueEdit == null)
            {
                hasError = true;
                errorMessage = "Le nom, le symbole, le type et le niveau de risque doivent être renseignés pour pouvoir modifier un actif.";
                return;
            }

            ActifModele actifModifie = new ActifModele(selectedNomEdit, selectedSymboleEdit, selectedTypeEdit, selectedISINEdit, selectedNvRisqueEdit);

            actif.ModifierActif(actifModifie);

            SelectedActifEdit = "Aucun";

            LoadActifs();
        }

        public void Supprimer()
        {
            foreach(string nomActif in ListeActifASuppr)
            {
                actif.SupprimerActif(nomActif);
            }

            LoadActifs();
        }

        public void ChangerEtatSuppression(string nom)
        {
            if(ListeActifASuppr.Contains(nom))
            {
                ListeActifASuppr.Remove(nom);
            }
            else
            {
                ListeActifASuppr.Add(nom);
            }
        }
    }
}
