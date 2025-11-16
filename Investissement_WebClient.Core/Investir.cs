using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Investissement_WebClient.Core
{
    public class Investir
    {
        private readonly IInvestirSQLite _Iinvestir;

        public Investir(IInvestirSQLite Iinvestir)
        {
            _Iinvestir = Iinvestir;
        }

        public List<ModeleInvest> GetNomModeles()
        {
            return _Iinvestir.ReadNomModeles();
        }

        public List<string> GetNomActifs()
        {
            return  _Iinvestir.ReadNomActifs();
        }

        public List<TransactionModele> GetTransactionsModele(long idModele)
        {
            return _Iinvestir.ReadTransactionsModele(idModele);
        }

        public void AddInvest(List<Transaction> transactions)
        {
            _Iinvestir.AddInvest(transactions);

            _Iinvestir.ajouterInvestissementTotal(transactions[0].date, Convert.ToInt64(transactions.Sum(q => q.quantite)));
        }
    }
}
