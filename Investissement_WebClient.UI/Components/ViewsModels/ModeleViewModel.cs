using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ModeleViewModel
    {
        private readonly IActifService  _actifService;
        private readonly IModeleService _modeleService;
        
        public ModeleViewModel(IActifService actifService, ITransactionService transactionService, IModeleService modeleService)
        {
            _actifService = actifService;
            _modeleService = modeleService;
        }
        
        public string SelectedMode { get; set; } = "Ajouter";
        
        public IEnumerable<ModeleCompositionDto> ModelesComposition { get; set; } = [];
        
        public ModeleDto SelectedModele { get; set; } = new ModeleDto();
        public ModeleDto SelectedModeleEdit { get; set; } = new  ModeleDto();

        public List<TransactionDto> CompositionModele { get; set; } = [];
        public List<TransactionDto> CompositionModeleEdit { get; set; } = [];

        public IEnumerable<ActifDto> ActifEnregistre { get; set; } = [];

        public List<int> ModelesAsuppr { get; set; } = [];

        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        
        

        public async Task LoadData()
        {
            await LoadModeles();
            await LoadActifsEnregistres();
        }
        private async Task LoadModeles()
        {
            ModelesComposition = await _modeleService.GetModelesComposition();
        }

        private async Task LoadCompositionModele(int idModele)
        {
            CompositionModeleEdit = await _modeleService.GetCompositionModele(idModele);
        }

        private async Task LoadActifsEnregistres()
        {
            ActifEnregistre = await _actifService.GetActifsEnregistres();
        }
        
        public void ChangeMode()
        {
            SelectedMode = SelectedMode == "Ajouter" ? "Modifier" : "Ajouter";
        }

        public async Task OnChangeModeleEdit(ChangeEventArgs e)
        {
            if(int.TryParse(e?.Value.ToString(), out int idModeleEdit))
            {
                await LoadCompositionModele(idModeleEdit);
                SelectedModeleEdit =  new ModeleDto
                {
                   Id = idModeleEdit, 
                   Nom = ModelesComposition.First(m => m.Id == idModeleEdit).Nom
                };
            }
            else
            {
                SelectedModeleEdit = new ModeleDto();
            }
        }
        
        public void AddActifModele()
        {
            (SelectedMode == "Ajouter" ? CompositionModele : CompositionModeleEdit).Add(new TransactionDto());
        }

        public void DellActifModele(TransactionDto preparationTransaction)
        {
            (SelectedMode == "Ajouter" ? CompositionModele : CompositionModeleEdit).Remove(preparationTransaction);
        }

        private bool VerificationModeleCorrect(ModeleDto modele,List<TransactionDto> transactions)
        {
            if (string.IsNullOrWhiteSpace(modele.Nom))
            {
                ErrorMessage = "Entrez un nom pour votre modèle";
                return false;
            }

            if (ModelesComposition.Any(m => m.Nom == modele.Nom && m.Id != modele.Id))
            {
                ErrorMessage = "Un modèle de même nom existe déjà";
                return false;
            }

            if (transactions.Where(t => t.IdActif > 0).GroupBy(t => t.IdActif).Any(g => g.Count() > 1))
            {
                ErrorMessage = "Vous avez sélectionné plusieurs fois le même actif.";
                return false;
            }
            
            return true;
        }

        public async Task Ajouter()
        {
            if (!VerificationModeleCorrect(SelectedModele, CompositionModele))
            {
                HasError = true;
                return;
            }
                
            await _modeleService.AjouterModele(SelectedModele.Nom, CompositionModele);

            CompositionModele.Clear();
            SelectedModele = new  ModeleDto();
            await LoadModeles();
        }

        public async Task Editer()
        {
            if (!VerificationModeleCorrect(SelectedModeleEdit, CompositionModeleEdit))
            {
                HasError = true;
                return;
            }
            
            await _modeleService.UpdateModele(SelectedModeleEdit, CompositionModeleEdit);

            CompositionModeleEdit.Clear();
            SelectedModeleEdit = new ModeleDto();
            SelectedMode = "Ajouter";
            await LoadModeles();
        }

        public void ChangerEtatSuppression(int idModele)
        {
            if (!ModelesAsuppr.Remove(idModele)) ModelesAsuppr.Add(idModele);
        }

        public async Task Supprimer()
        {
            await _modeleService.DeleteModeles(ModelesAsuppr);
            
            ModelesAsuppr.Clear();
            await LoadModeles();
        }
    }
}
