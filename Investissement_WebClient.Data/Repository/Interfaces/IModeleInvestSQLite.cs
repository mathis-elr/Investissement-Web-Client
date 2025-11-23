using Investissement_WebClient.Data.Modeles;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IModeleInvestSQLite
    {
        public List<string> ReadNomsActif();

        public List<string> ReadModeles();

        public List<TransactionModele> ReadTransactionsModele(string modele);

        public void AjouterModele(List<TransactionModele> transactionsModele);

        public void SupprimerModele(string modele);
    }
}
