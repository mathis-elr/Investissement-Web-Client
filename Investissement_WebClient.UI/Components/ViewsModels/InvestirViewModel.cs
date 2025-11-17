using Investissement_WebClient.Core;
using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Investissement_WebClient.Data.Repository.SQLite;
using Microsoft.Data.Sqlite;
using System;
using System.Reflection;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class InvestirViewModel
    {
        public Investir investir;
        public DateTime selectedDate { get; set; }
        private long selectedModele;
        private string selectedActif;

        public List<ModeleInvest> ListeModeles { get; set; } = new List<ModeleInvest>();
        public List<string> ListeActifs { get; set; } = new List<string>();
        public List<TransactionModele> ListeTransactionsModele { get; set; } = new List<TransactionModele>();
        public List<Transaction> ListeTransactions { get; set; } = new List<Transaction>();

        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = string.Empty;

        public long SelectedModele
        {
            get {  return selectedModele; }
            set
            { 
                selectedModele = value;
                LoadActifsModele(selectedModele);
            }
        }

        public string SelectedActif
        {
            get { return selectedActif; }
            set
            {
                selectedActif = value;
            }
        }

        public InvestirViewModel() 
        {
            IInvestirSQLite Iinvestir = new InvestirSQLite(BDDService.ConnectionString);
            investir = new Investir(Iinvestir);

            selectedDate = DateTime.Now;
            SelectedModele = -1;
            LoadModeles();
            LoadActifs();
        }

        public void LoadModeles()
        {
            ListeModeles = investir.GetNomModeles();
        }

        public void LoadActifs()
        {
            ListeActifs = investir.GetNomActifs();  
        }

        public void LoadActifsModele(long modele)
        {
            ListeTransactionsModele = investir.GetTransactionsModele(modele);

            ListeTransactions.Clear();
            ListeTransactions.AddRange(
                ListeTransactionsModele.Select(tm => new Transaction(
                    selectedDate,
                    tm.actif,
                    tm.quantite.HasValue ? (double?)tm.quantite.Value : null,
                    null // prix
                ))
            );
        }

        public void AddActifInvest()
        {
            ListeTransactions.Add(new Transaction(selectedDate, ListeActifs.First()));
        }

        public void DellActifInvest(Transaction transaction)
        {
            ListeTransactions.Remove(transaction);
        }

        public void Investir()
        {
            hasError= false;
            errorMessage= string.Empty;

            if (ListeTransactions.Count == 0)
            {
                hasError = true;
                errorMessage = "Selectionner au moins un actif pour pouvoir investir";
                return;
            }
            if(ListeTransactions.Any(t => t.quantite == null || t.prix == null || t.quantite == 0 || t.prix == 0 || t.quantite < 0 || t.prix < 0))
            {
                hasError = true;
                errorMessage = "La quantité et le prix doivent être des valeur valides (supérieures à 0 et non null).";
                return;
            }

            try
            {
                investir.AddInvest(ListeTransactions);
            }
            catch (SqliteException)
            {
                hasError = true;
                errorMessage = "Une erreur est survenue lors de l'ajout de l'investissement.";
                return;
            }
            catch (Exception ex)
            {
                hasError = true;
                errorMessage = ex.Message;
                return;
            }

            selectedModele = -1;
            ListeTransactions = [];
        }
    }
}
