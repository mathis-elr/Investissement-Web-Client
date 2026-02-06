using Investissement_WebClient.Core.InterfacesServices;
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
        public IEnumerable<PreparationTransaction> ActifsModele { get; set; }
        public List<PreparationTransaction> TransactionsInvestissement { get; set; } = new ();
        
        public DateTime SelectedDate { get; set; } = DateTime.Now;
        private int SelectedModele { get; set; }
        public DateTime? DateDernierInvest { get; set; }
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        
        
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
        // public void LoadDernierInvestissement()
        // {
        //     ListeDernierInvestissement = _serviceInvestir.GetDernierInvest();
        //     dateDernierInvest = ListeDernierInvestissement?.FirstOrDefault()?.date;
        // }
        private async Task LoadCompositionModele(int idModele)
        {
            ActifsModele = await _serviceInvestir.GetCompositionModele(idModele);

            TransactionsInvestissement.Clear();
            TransactionsInvestissement.AddRange(
                ActifsModele.Select(tm => new PreparationTransaction(
                    tm.IdActif,  
                    tm.NomActif,  
                    tm.Quantite,  
                    null           
                ))
            );
        }
        
        public async Task OnChangeModele(ChangeEventArgs e)
        {
            if (int.TryParse(e?.Value.ToString(), out int idModele))
            {
                SelectedModele = idModele;
                await LoadCompositionModele(SelectedModele);
            }
        }

        public void AddActifInvest()
        {
            TransactionsInvestissement.Add(new PreparationTransaction(
                ActifsEnregistre.Select(a => a.Id).First(),
                ActifsEnregistre.Select(a => a.Nom).First(),
                null,
                null));
        }
        public void DellActifInvest(PreparationTransaction transaction)
        {
            TransactionsInvestissement.Remove(transaction);
        }

        public async Task Investir()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            // if (ListeTransactions.Count == 0)
            // {
                // hasError = true;
                // errorMessage = "Selectionner au moins un actif pour pouvoir investir";
                // return;
            // }
            // if(ListeTransactions.Any(t => t.quantite == null || t.prix == null || t.quantite == 0 || t.prix == 0 || t.quantite < 0 || t.prix < 0))
            // {
                // hasError = true;
                // errorMessage = "La quantité et le prix doivent être des valeur valides (supérieures à 0 et non null).";
                // return;
            // }

            // try
            // {
                // investir.AddInvest(ListeTransactions);
            // }
            // catch (SqliteException)
            // {
                // hasError = true;
                // errorMessage = "Une erreur est survenue lors de l'ajout de l'investissement.";
                // return;
            // }
            // catch (Exception ex)
            // {
                // hasError = true;
                // errorMessage = ex.Message;
                // return;
            // }

            // selectedModele = "Aucun";
            // ListeTransactions = [];
            // LoadDernierInvestissement();
        }
    }
}
