using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using System.Reflection.Metadata;

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
            DateTime datePremierInvest = _Iinvestir.getDatePremierInvest();
            DateTime dateDuJour = DateTime.Now;

            if (dateInvest > dateDuJour)
            {
                throw new Exception("La date de la transaction ne peut pas être dans le futur.");
            }

            double montant = this.calculerInvestissementTotal(transactions);
            _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);

            if (dateInvest < dateDuJour)
            {
                this.ajouterInvestissementsTotauxManquant(Convert.ToDateTime(_Iinvestir.getDateDernierInvest()),dateDuJour);
            }

            if (dateInvest < datePremierInvest)
            {
                this.changerDatePremierInvest(dateInvest);
                this.changerAncienPremierInvest(datePremierInvest, montant);
                this.ajouterInvestissementsTotauxManquant(dateInvest, datePremierInvest);
                this.modifierInvestissementsTotauxSuivant(datePremierInvest, montant);
            }
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

        private void ajouterInvestissementsTotauxManquant(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut != dateFin)
            {
                DateTime dateCourante = Convert.ToDateTime(dateDebut).AddDays(1); //+1j car on a dejà la date du dernierInvest

                while (dateCourante < dateFin)
                {
                    _Iinvestir.ajouterInvestissementTotal(dateCourante, 0);
                    dateCourante = dateCourante.AddDays(1);
                }
            }
        }

        private void changerDatePremierInvest(DateTime dateInvest)
        {
            DateTime datePremierInvest = dateInvest.AddDays(-1);
            _Iinvestir.ajouterInvestissementTotal(datePremierInvest, 0);
        }

        private void changerAncienPremierInvest(DateTime datePremierInvest, double montant)
        {
            _Iinvestir.modifierInvestissementTotal(datePremierInvest, montant);
        }

        private void modifierInvestissementsTotauxSuivant(DateTime datePremierInvest, double montant)
        {
            DateTime dateDernierInvest = Convert.ToDateTime(_Iinvestir.getDateDernierInvest());
            DateTime dateCourante = datePremierInvest.AddDays(1);

            while (dateCourante < dateDernierInvest)
            {
                _Iinvestir.modifierInvestissementTotal(dateCourante, montant);
                dateCourante = dateCourante.AddDays(1);
            }
        }
    }
}
