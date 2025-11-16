using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IInvestirSQLite
    {
        public List<ModeleInvest> ReadNomModeles();

        public List<string> ReadNomActifs();

        public List<TransactionModele> ReadTransactionsModele(long idModele);

        public void AddInvest(List<Transaction> transactions);

        public double getQuantiteTotalePrecedente(DateTime date);

        public void ajouterInvestissementTotal(DateTime date, double quantiteInvestit);
    }
}
