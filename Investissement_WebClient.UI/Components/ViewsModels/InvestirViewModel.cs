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
        private readonly IPatrimoineService _patrimoineService;
        
        public InvestirViewModel(IInvestirService investirService, IActifService actifService, IModeleService modeleService, IPatrimoineService patrimoineService)
        {
            _investirService = investirService; 
            _actifService = actifService;
            _modeleService = modeleService;
            _patrimoineService = patrimoineService;
        }

        public IEnumerable<ModeleDto> Modeles { get; set; } = [];
        public IEnumerable<ActifDto> ActifsEnregistre { get; set; } = [];
        private IEnumerable<TransactionDto> ActifsModele { get; set; } = [];
        public IEnumerable<InvestissementGetDto> Investissements { get; set; } = [];
        public List<TransactionDto> TransactionsInvestissement { get; set; } = new ();
        
        public DateTime SelectedDateInvest { get; set; } = DateTime.Now;
        public int? SelectedIdModele { get; set; } = null;
        public string? NoteInvestissement { get; set; } = null;
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
            else
            {
                SelectedIdModele = null;
                NoteInvestissement = null;
                ActifsModele = [];
                TransactionsInvestissement = [];
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
            if (TransactionsInvestissement.Any(t => t.Quantite == null || t.Prix == null || t.Quantite <= 0 || t.Prix <= 0))
            {
                HasError = true;

                ErrorMessage = "La quantité et le prix doivent être des valeur valides (supérieures à 0 et champs obligatoires).";
                return;
            }

            InvestissementDto investissementDto = new InvestissementDto
            {
                Date = SelectedDateInvest,
                idModele = SelectedIdModele,
                Note = NoteInvestissement,
                Transactions = TransactionsInvestissement
            };

            await _investirService.SaveInvestissement(investissementDto);

            NoteInvestissement = null;
            SelectedIdModele = null;
            TransactionsInvestissement = [];
            await LoadInvestissements();
        }

        public async Task DeleteDernierInvest(InvestissementGetDto investissement)
        {
            await _investirService.DeleteDernierInvest(investissement);
            await _patrimoineService.DeleteHistoriquePatrimoinePeriode(investissement.DateInvest);
            await LoadInvestissements();
        }
    }
} 
