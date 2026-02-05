using Investissement_WebClient.Core;
using System.Diagnostics;
using Investissement_WebClient.Core.Modeles;


namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ActifViewModel
    {
        private readonly ActifEnregistre actif;

        public Actif actifSelectionne { get; set; } = new();
        private string nomActifSelectionne;

        public string? selectedNom { get; set; } = null;
        public string? selectedSymbole { get; set; } = null;
        public ActifType? selectedType { get; set; } = null;
        public string? selectedISIN { get; set; } = null;
        public ActifRisque? selectedNvRisque { get; set; } = null;
        
        public string selectedMode { get; set; }
        public List<string> ListeModes { get; set; }

        public List<string>? ListeNvRisque { get; set; }
        public List<Actif> ListeActifs { get; set; }
        public List<ActifEnregistre>? ListeActifsEnregistre { get; set; }
        public List<string>? ListeNomsActifs { get; set; }

        private ActifEnregistre? selectActifModelEdit { get; set; } = null;
        private string selectedActifEdit { get; set; }
        public string? selectedNomEdit { get; set; } = null;
        public string? selectedSymboleEdit { get; set; } = null;
        public ActifType? selectedTypeEdit { get; set; } = null;
        public string? selectedISINEdit { get; set; } = null;
        public ActifRisque? selectedNvRisqueEdit { get; set; } = null;

        public List<string> ListeActifASuppr { get; set; } = new List<string>();

        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = string.Empty;

        public string NomActifSelectionne
        {
            get { return nomActifSelectionne; }
            set
            {
                nomActifSelectionne = value;
                actifSelectionne = ListeActifs.FirstOrDefault(a => a.Name == value);
                switch (actifSelectionne.Type)
                {
                    case "ETC":
                        selectedNvRisque = ActifRisque.Faible;
                        break;
                    case "ETF":
                        selectedNvRisque = ActifRisque.Moyen;
                        break;
                    case "Action":
                        selectedNvRisque = ActifRisque.Fort;
                        break;
                    case "Crypto":
                        selectedNvRisque = ActifRisque.Fort;
                        break;
                }
            }
        }

        public string SelectedActifEdit
        {
            get {  return selectedActifEdit; }
            set
            {
                selectedActifEdit = value;
                if(value != "Aucun")
                {
                    selectActifModelEdit = ListeActifsEnregistre.FirstOrDefault(actif => actif.Nom == value);
                    selectedNomEdit = selectActifModelEdit.Nom;
                    selectedSymboleEdit = selectActifModelEdit.Symbole;
                    selectedTypeEdit = selectActifModelEdit.Type;
                    selectedISINEdit = selectActifModelEdit.Isin;
                    selectedNvRisqueEdit = selectActifModelEdit.Risque;
                }
                else
                {
                    selectActifModelEdit = null;
                    selectedNomEdit = string.Empty;
                    selectedSymboleEdit = string.Empty;
                    selectedTypeEdit = null;
                    selectedISINEdit = null;
                    selectedNvRisqueEdit = null;
                }
            }
        }

        public void LoadNvRisque()
        {
            // ListeNvRisque = actif.GetListeNvRisque();
        }
        public void LoadActifs()
        {
            // ListeActifs = actif.GetListeActifs();
        }
        public void LoadActifsEnregistre()
        {
            // ListeActifsEnregistre = actif.GetListeActifsEnresgistre();
            // ListeNomsActifs = ListeActifsEnregistre.Select(nom => nom.nom).ToList();
        }

        public void LoadModes()
        {
            // ListeModes = actif.GetModes();
            // selectedMode = ListeModes.First();
        }

        public ActifViewModel() 
        {
            // IActifEnregistreSQLite Iactif = new ActifEnregistreSqLite(BDDService.ConnectionString);
            // actif = new Actif(Iactif);

            LoadModes();
            LoadNvRisque();
            LoadActifsEnregistre();
            LoadActifs();

            selectedActifEdit = "Aucun";
        }

        public void ChangeMode()
        {
            selectedMode = selectedMode == "Ajouter" ? "Modifier" : "Ajouter";
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

            // ActifEnresgistreModele selectActifEnresgistreModelAdd = new ActifEnresgistreModele(selectedNom, selectedSymbole, selectedType, selectedISIN, selectedNvRisque);
            // actif.AjouterActif(selectActifEnresgistreModelAdd);

            selectedNom = null;
            selectedSymbole = null;
            selectedType = null;
            selectedISIN = null;
            selectedNvRisque = null;

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

            // ActifEnresgistreModele actifEnresgistreModifie = new ActifEnresgistreModele(selectedNomEdit, selectedSymboleEdit, selectedTypeEdit, selectedISINEdit, selectedNvRisqueEdit);

            // actif.ModifierActif(actifEnresgistreModifie);

            SelectedActifEdit = "Aucun";

            LoadActifs();
        }

        public void Supprimer()
        {
            foreach(string nomActif in ListeActifASuppr)
            {
                // actif.SupprimerActif(nomActif);
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
