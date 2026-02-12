using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ModeleViewModel
    {
        private readonly IServiceActif  _serviceActif;
        private readonly IServiceInvestir _serviceInvestir;
        
        public ModeleViewModel(IServiceActif serviceActif, IServiceInvestir serviceInvestir)
        {
            _serviceActif = serviceActif;
            _serviceInvestir = serviceInvestir;
        }
        
        public string SelectedMode { get; set; } = "Ajouter";
        
        public IEnumerable<ItemDto> Modeles { get; set; } = [];
        
        public ItemDto SelectedItem { get; set; } = new ItemDto();
        public ItemDto SelectedItemEdit { get; set; } = new  ItemDto();

        public List<TransactionDto> CompositionModele { get; set; } = [];
        public List<TransactionDto> CompositionModeleEdit { get; set; } = [];

        public IEnumerable<ItemDto> ActifEnregistre { get; set; } = [];

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
            Modeles = await _serviceInvestir.GetModeles();
        }

        private async Task LoadCompositionModele(int idModele)
        {
            CompositionModeleEdit = await _serviceInvestir.GetCompositionModele(idModele);
        }

        private async Task LoadActifsEnregistres()
        {
            ActifEnregistre = await _serviceActif.GetActifsEnregistres();
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
                SelectedItemEdit =  new ItemDto
                {
                   Id = idModeleEdit, 
                   Nom = Modeles.First(m => m.Id == idModeleEdit).Nom
                };
            }
            else
            {
                SelectedItemEdit = new ItemDto();
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

        private void VerificationModeleCorrect()
        {
            HasError = true;

            if (string.IsNullOrWhiteSpace(SelectedItem.Nom))
            {
                ErrorMessage = "Entrez un nom pour votre modèle";
                return;
            }

            if (Modeles.Any(m => m.Nom == SelectedItem.Nom && m.Id != SelectedItem.Id))
            {
                ErrorMessage = "Un modèle de même nom existe déjà";
                return;
            }

            if (CompositionModele.Where(t => t.IdActif > 0).GroupBy(t => t.IdActif).Any(g => g.Count() > 1))
            {
                ErrorMessage = "Vous avez sélectionné plusieurs fois le même actif.";
                return;
            }

            HasError = false;
            ErrorMessage = string.Empty;
        }

        public async Task Ajouter()
        {
            VerificationModeleCorrect();
                
            await _serviceInvestir.AjouterModele(SelectedItem.Nom, CompositionModele);

            CompositionModele.Clear();
            SelectedItem = new  ItemDto();
            await LoadModeles();
        }

        public async Task Editer()
        {
            VerificationModeleCorrect();

            await _serviceInvestir.UpdateModele(SelectedItemEdit, CompositionModeleEdit);

            CompositionModeleEdit.Clear();
            SelectedItemEdit = new ItemDto();
            SelectedMode = "Ajouter";
            await LoadModeles();
        }

        public void ChangerEtatSuppression(int idModele)
        {
            if (!ModelesAsuppr.Remove(idModele)) ModelesAsuppr.Add(idModele);
        }

        public async Task Supprimer()
        {
            await _serviceInvestir.DeleteModeles(ModelesAsuppr);
            
            ModelesAsuppr.Clear();
            await LoadModeles();
        }
    }
}
