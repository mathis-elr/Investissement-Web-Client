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
        private readonly IServiceActif _serviceActif;
        
        public string SelectedMode { get; set; } = "Ajouter";
        
        public Actif SelectedActif { get; set; } = new Actif();
        
        public ActifEnregistre SelectedActifEdit { get; set; } = new ActifEnregistre();
        
        public IEnumerable<string> NiveauxRisqueActif { get; } = Enum.GetNames(typeof(ActifRisque));
        public IEnumerable<string> TypesActif { get; } = Enum.GetNames(typeof(ActifType));
        public IEnumerable<ItemDto> ActifsDisponibles { get; set; } = [];
        public IEnumerable<ItemDto> ActifsEnregistre { get; set; } = [];
        public List<int> ActifASuppr { get; set; } = [];
        
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        
        
        public ActifViewModel(IServiceActif  serviceActif) 
        {
            _serviceActif = serviceActif;
        }
        
        
        private async Task LoadActifsEnregistre()
        {
            ActifsEnregistre = await _serviceActif.GetActifsEnregistres();
        }

        private async Task LoadActifsDisponibles()
        {
            ActifsDisponibles = await _serviceActif.GetActifsDisponibles();
        }

        private async Task LoadActif(int idActif)
        {
            SelectedActif = await _serviceActif.GetActifDisponible(idActif);
        }
        
        private async Task LoadActifEdit(int idActif)
        {
            SelectedActifEdit = await _serviceActif.GetActifEnregistre(idActif);
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
                await LoadActif(idActif);
            }
            else
            {
                SelectedActif = new Actif();
            }
        }

        public async Task OnChangeActifEdit(ChangeEventArgs e)
        {
            if(int.TryParse(e?.Value.ToString(), out int idActifEdit))
            {
                await LoadActifEdit(idActifEdit);
            }
            else
            {
                SelectedActifEdit = new ActifEnregistre();
            }
        }

        public void ChangeMode()
        {
            SelectedMode = SelectedMode == "Ajouter" ? "Modifier" : "Ajouter";
        }

        public async Task Ajouter()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if(SelectedActif.Nom == null|| SelectedActif.Symbole == null || SelectedActif.Type == null)
            {
                HasError = true;
                ErrorMessage = "Le nom, le symbole, le type et le niveau de risque doivent être renseignés pour pouvoir ajouter un actif.";
                return;
            }
            
            await _serviceActif.AjouterActif(SelectedActif);

            SelectedActif = new Actif();

            await LoadActifsEnregistre();
            await LoadActifsDisponibles();
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
            
            await _serviceActif.ModifierActif(SelectedActifEdit);

            SelectedActifEdit = new ActifEnregistre();

            await LoadActifsEnregistre();
            await LoadActifsDisponibles();
        }

        public async Task Supprimer()
        { 
            await _serviceActif.SupprimerActifs(ActifASuppr);

            ActifASuppr.Clear();
            await LoadActifsEnregistre();
            await LoadActifsDisponibles();
        }

        public void ChangerEtatSuppression(int id)
        {
            if(ActifASuppr.Contains(id))
            {
                ActifASuppr.Remove(id);
            }
            else
            {
                ActifASuppr.Add(id);
            }
        }
    }
}
