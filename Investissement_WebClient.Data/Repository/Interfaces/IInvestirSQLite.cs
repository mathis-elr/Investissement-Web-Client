using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IInvestirSQLite
    {
        public List<string> ReadNomModeles();

        public List<string> ReadNomActifs();

        public List<TransactionModele> ReadTransactionsModele(string modele);

        public List<Transaction> GetTransactionsDernierInvest();

        public DateTime? getDateDernierInvest();

        public double getQuantiteTotalePrecedente(DateTime date);

        public DateTime getDatePremierInvest();

        public void AddInvest(List<Transaction> transactions);

        public void ajouterInvestissementTotal(DateTime date, double quantiteInvestit);

        public void modifierInvestissementTotal(DateTime date, double quantiteInvestit);
    }
}
