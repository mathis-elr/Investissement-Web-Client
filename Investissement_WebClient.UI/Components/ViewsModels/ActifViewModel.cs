using Investissement_WebClient.Core;
using System.Diagnostics;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;


namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ActifViewModel
    {
        private readonly IActifService _actifService;
        
        public string SelectedMode { get; set; } = "Enregistrer";
        
        public ActifDto SelectedActif { get; set; } = new ActifDto();
        
        public ActifDto SelectedActifEdit { get; set; } = new ActifDto();
        
        public IEnumerable<string> NiveauxRisqueActif { get; } = Enum.GetNames(typeof(ActifRisque));
        public IEnumerable<string> TypesActif { get; } = Enum.GetNames(typeof(ActifType));
        public IEnumerable<ActifDto> ActifsDisponibles { get; set; } = [];
        public IEnumerable<ActifDto> ActifsEnregistre { get; set; } = [];

        public List<ActifDto> ActifASuppr { get; set; } = [];
        
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        
        
        public ActifViewModel(IActifService  actifService) 
        {
            _actifService = actifService;
        }
        
        
        private async Task LoadActifsEnregistre()
        {
            ActifsEnregistre = await _actifService.GetActifsEnregistres();
        }

        private async Task LoadActifsDisponibles()
        {
            ActifsDisponibles = await _actifService.GetActifsDisponibles();
        }

        public async Task LoadData()
        {
            await LoadActifsDisponibles();
            await LoadActifsEnregistre();
        }

        public async Task OnChangeActif(ChangeEventArgs e)
        {
            if(int.TryParse(e?.Value.ToString(), out int idActif))
            {
                SelectedActif = ActifsDisponibles.Where(a => a.Id == idActif).FirstOrDefault();
            }
            else
            {
                SelectedActif = new ActifDto();
            }
        }

        public async Task OnChangeActifEdit(ChangeEventArgs e)
        {
            if(int.TryParse(e?.Value.ToString(), out int idActifEdit))
            {
                SelectedActifEdit = ActifsEnregistre.Where(a => a.Id == idActifEdit).FirstOrDefault();
            }
            else
            {
                SelectedActifEdit = new ActifDto();
            }
        }

        public void ChangeMode()
        {
            SelectedMode = SelectedMode == "Enregistrer" ? "Modifier" : "Enregistrer";
        }

        public async Task Enregistrer()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if(SelectedActif.Nom == null|| SelectedActif.Symbole == null || SelectedActif.Type == null)
            {
                HasError = true;
                ErrorMessage = "Le nom, le symbole, le type et le niveau de risque doivent être renseignés pour pouvoir ajouter un actif.";
                return;
            }

            if (ActifsEnregistre.Any(a => a.Nom == SelectedActif.Nom))
            {
                HasError = true;
                ErrorMessage = "Vous avez déjà enregistré cet actif";
                return;
            }
            
            await _actifService.EnregistrerActif(SelectedActif);

            SelectedActif = new ActifDto();

            await LoadData();
        }

        public async Task Editer()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (SelectedActifEdit.Nom == null || SelectedActifEdit.Symbole == null || SelectedActifEdit.Type == null || SelectedActifEdit.Risque == null)
            {
                HasError = true;
                ErrorMessage = "Le nom, le symbole, le type et le niveau de risque doivent être renseignés pour pouvoir modifier un actif.";
                return;
            }
            
            await _actifService.ModifierActif(SelectedActifEdit);

            SelectedActifEdit = new ActifDto();

            await LoadData();
        }

        public async Task Supprimer()
        { 
            await _actifService.SupprimerActifs(ActifASuppr);

            EffacerSelectionSuppresion();
            await LoadData();
        }

        public void ChangerEtatSuppression(ActifDto actif)
        {
            if(ActifASuppr.Contains(actif))
            {
                ActifASuppr.Remove(actif);
            }
            else
            {
                ActifASuppr.Add(actif);
            }
        }

        public void EffacerSelectionSuppresion()
        {
            ActifASuppr.Clear();
        }
    }
}
