using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.AspNetCore.Components;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestirViewModel
    {
        private readonly IServiceInvestir _serviceInvestir;
        private readonly IServiceActif _serviceActif;
        
        public InvestirViewModel(IServiceInvestir serviceInvestir, IServiceActif serviceActif)
        {
            _serviceInvestir = serviceInvestir; 
            _serviceActif = serviceActif;
        }
        
        public IEnumerable<(int Id, string Nom)> Modeles { get; set; }
        public IEnumerable<(int Id, string Nom)> ActifsEnregistre { get; set; }
        private IEnumerable<PreparationTransaction> ActifsModele { get; set; }
        public List<PreparationTransaction> TransactionsInvestissement { get; set; } = new ();
        
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
            Modeles = await _serviceInvestir.GetModeles();
        }
        private async Task LoadActifs()
        {
            ActifsEnregistre = await _serviceActif.GetNomActifsEnregistres();  
        }
        private async Task LoadCompositionModele(int idModele)
        {
            ActifsModele = await _serviceInvestir.GetCompositionModele(idModele);

            TransactionsInvestissement.Clear();
            TransactionsInvestissement.AddRange(
                ActifsModele.Select(tm => new PreparationTransaction
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
            var premierActif = ActifsEnregistre.FirstOrDefault();

            TransactionsInvestissement.Add(new PreparationTransaction
            {
                IdActif = premierActif.Id,
                NomActif = premierActif.Nom,
                Quantite = null,
                Prix = null
            });
        }
        public void DellTransactionInvest(PreparationTransaction transaction)
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
               await  _serviceInvestir.SaveInvestissement(SelectedIdModele, SelectedDateInvest, TransactionsInvestissement);
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
