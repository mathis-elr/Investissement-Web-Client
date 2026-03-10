using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestirViewModel
    {
        private readonly IInvestirService _investirService;
        private readonly IActifService _actifService;
        private readonly IModeleService _modeleService;
        
        public InvestirViewModel(IInvestirService investirService, IActifService actifService, IModeleService modeleService)
        {
            _investirService = investirService; 
            _actifService = actifService;
            _modeleService = modeleService;
        }

        public IEnumerable<ModeleDto> Modeles { get; set; } = [];
        public IEnumerable<ActifDto> ActifsEnregistre { get; set; } = [];
        private IEnumerable<TransactionDto> ActifsModele { get; set; } = [];
        public IEnumerable<InvestissementDto> Investissements { get; set; } = [];
        public List<TransactionDto> TransactionsInvestissement { get; set; } = new ();
        
        public DateTime SelectedDateInvest { get; set; } = DateTime.Now;
        private int? SelectedIdModele { get; set; } = null;
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        
        
        public async Task LoadData()
        {
            await LoadActifs();
            await LoadModeles();
            await LoadInvestissements();
        }

        private async Task LoadModeles()
        {
            Modeles = await _modeleService.GetModeles();
        }
        private async Task LoadActifs()
        {
            ActifsEnregistre = await _actifService.GetActifsEnregistres();  
        }
        private async Task LoadCompositionModele(int idModele)
        {
            ActifsModele = await _modeleService.GetCompositionModele(idModele);

            TransactionsInvestissement.Clear();
            TransactionsInvestissement.AddRange(
                ActifsModele.Select(tm => new TransactionDto
                {
                    IdActif = tm.IdActif,
                    NomActif = tm.NomActif,
                    Quantite = tm.Quantite,
                    Prix = null
                })
            );
        }

        private async Task LoadInvestissements()
        {
            Investissements = await _investirService.GetInvestissements();
        }
        
        public async Task OnChangeModele(ChangeEventArgs e)
        {
            if (int.TryParse(e?.Value.ToString(), out int idModele))
            {
                SelectedIdModele = idModele;
                int modeleNotNull = idModele;
                await LoadCompositionModele(modeleNotNull);
            }
        }

        public void AddTransactionInvest()
        {
            TransactionsInvestissement.Add(new TransactionDto());
        }
        public void DellTransactionInvest(TransactionDto transaction)
        {
            TransactionsInvestissement.Remove(transaction);
        }

        public async Task Investir()
        {
            if (TransactionsInvestissement.Count == 0)
            {
                HasError = true;
                ErrorMessage = "Selectionner au moins un actif pour pouvoir investir";
                return;
            }
            if (SelectedDateInvest > DateTime.Now)
            {
                HasError = true;
                ErrorMessage = "Impossible d'investir dans le futur";
                return;
            }

            try
            {
               await  _investirService.SaveInvestissement(SelectedIdModele, SelectedDateInvest, TransactionsInvestissement);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                return;
            }

            SelectedIdModele = null;
            TransactionsInvestissement = [];
            await LoadInvestissements();
        }
    }
}
