using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;

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

            ajouterInvestissementTotal(transactions);
        }

        private void ajouterInvestissementTotal(List<Transaction> transactions)
        {
            DateTime? dateDernierInvest = _Iinvestir.getDateDernierInvest();

            this.verifierExistanceDernierInvestissement(dateDernierInvest);

            DateTime dateInvest = transactions[0].date;
            if (dateInvest > DateTime.Now)
            {
                throw new Exception("La date de la transaction ne peut pas être dans le futur.");
            }

            double montant = this.calculerInvestissementTotal(transactions);
            if (dateInvest < DateTime.Now)
            {
                this.ajouterInvestissementsTotauxManquant();
            }
            _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);
        }

        private void verifierExistanceDernierInvestissement(DateTime? date)
        {
            if (date == null)
            {
                _Iinvestir.ajouterInvestissementTotal(DateTime.Today.AddDays(-1), 0); //on ajoute au jour precedent ajd
            }
        }

        private double calculerInvestissementTotal(List<Transaction> transactions)
        {
            return Convert.ToDouble(transactions.Sum(q => q.quantite * q.prix));
        }

        private void ajouterInvestissementsTotauxManquant()
        {
            var dateDernierInvest = _Iinvestir.getDateDernierInvest();
            DateTime dateDuJour = DateTime.Today;

            if (dateDernierInvest != dateDuJour)
            {
                DateTime dateCourante = Convert.ToDateTime(dateDernierInvest).AddDays(1); //+1j car on a dejà la date du dernierInvest

                while (dateCourante < dateDuJour)
                {
                    _Iinvestir.ajouterInvestissementTotal(dateCourante, 0);
                    dateCourante = dateCourante.AddDays(1);
                }
            }
        }
    }
}
