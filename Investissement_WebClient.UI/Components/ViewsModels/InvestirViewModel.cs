using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestirViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IActifService _actifService;
        private readonly IModeleService _modeleService;
        
        public InvestirViewModel(ITransactionService transactionService, IActifService actifService, IModeleService modeleService)
        {
            _transactionService = transactionService; 
            _actifService = actifService;
            _modeleService = modeleService;
        }
        
        public IEnumerable<ModeleDto> Modeles { get; set; }
        public IEnumerable<ActifDto> ActifsEnregistre { get; set; }
        private IEnumerable<TransactionDto> ActifsModele { get; set; }
        public List<TransactionDto> TransactionsInvestissement { get; set; } = new ();
        
        public DateTime SelectedDateInvest { get; set; } = DateTime.Now;
        private int? SelectedIdModele { get; set; } = null;
        public bool HasError { get; private set; } = false;
        public string ErrorMessage { get; private set; } = string.Empty;
        
        
        public async Task LoadData()
        {
            await LoadActifs();
            await LoadModeles();
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
            HasError = false;
            ErrorMessage = string.Empty;

            if (TransactionsInvestissement.Count == 0)
            {
                HasError = true;
                ErrorMessage = "Selectionner au moins un actif pour pouvoir investir";
                return;
            }
            if(TransactionsInvestissement.Any(t => t.Quantite == null || t.Prix == null ||  t.Quantite <= 0 || t.Prix <= 0))
            {
                HasError = true;
                ErrorMessage = "La quantité et le prix doivent être des valeur valides (supérieures à 0 et non null).";
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
               await  _transactionService.SaveInvestissement(SelectedIdModele, SelectedDateInvest, TransactionsInvestissement);
            }
            catch (Exception ex)
            {
                HasError = true;
                Console.WriteLine(ex.Message);
                ErrorMessage = "Erreur d'insertion";
                return;
            }

            SelectedIdModele = null;
            TransactionsInvestissement = [];
            // LoadDernierInvestissement();
        }
        
        // public void LoadDernierInvestissement()
        // {
        //     ListeDernierInvestissement = _serviceInvestir.GetDernierInvest();
        //     dateDernierInvest = ListeDernierInvestissement?.FirstOrDefault()?.date;
        // }
    }
}
