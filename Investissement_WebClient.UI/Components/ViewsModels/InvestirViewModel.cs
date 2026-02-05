using Investissement_WebClient.Core;
using Microsoft.Data.Sqlite;
using System;
using System.Reflection;
using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestirViewModel
    {
        public List<Transaction> ListeTransactions { get; set; }
        public DateTime selectedDate { get; set; }

        private string selectedModele;

        public List<string> ListeModeles { get; set; } = new List<string>();
        public List<string> ListeActifs { get; set; } = new List<string>();

        public DateTime? dateDernierInvest { get; set; }

        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = string.Empty;

        public string SelectedModele
        {
            get {  return selectedModele; }
            set
            { 
                selectedModele = value;
                LoadActifsModele(selectedModele);
            }
        }

        public InvestirViewModel() 
        {

            selectedDate = DateTime.Now;
            SelectedModele = "Aucun";
            LoadModeles();
            LoadActifs();
            LoadDernierInvestissement();
        }

        public void LoadModeles()
        {
            // ListeModeles = investir.GetNomModeles();
        }
        public void LoadActifs()
        {
            // ListeActifs = investir.GetNomActifs();  
        }
        public void LoadDernierInvestissement()
        {
            // ListeDernierInvestissement = investir.GetDernierInvest();
            // dateDernierInvest = ListeDernierInvestissement?.FirstOrDefault()?.date;
        }
        public void LoadActifsModele(string modele)
        {
            // ListeTransactionsModele = investir.GetTransactionsModele(modele);

            // ListeTransactions.Clear();
            // ListeTransactions.AddRange(
                // ListeTransactionsModele.Select(tm => new Transaction(
                    // selectedDate,
                    // tm.actif,
                    // tm.quantite.HasValue ? (double?)tm.quantite.Value : null,
                    // null 
                // ))
            // );
        }

        public void AddActifInvest()
        {
            // ListeTransactions.Add(new Transaction(selectedDate, ListeActifs.First()));
        }
        public void DellActifInvest(Transaction transaction)
        {
            // ListeTransactions.Remove(transaction);
        }

        public async Task Investir()
        {
            hasError= false;
            errorMessage= string.Empty;

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
