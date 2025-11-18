using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using System.ComponentModel.Design;
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
            DateTime datePremierInvest = _Iinvestir.getDatePremierInvest();
            DateTime? dateDernierInvest = _Iinvestir.getDateDernierInvest();
            DateTime dateInvest = transactions[0].date.Date;
            DateTime dateDuJour = DateTime.Today;

            //cas 1 : On investit pour la première fois
            this.verifierExistanceDernierInvestissement(dateDernierInvest);

            DateTime dateDernierInvestNonNull = Convert.ToDateTime(_Iinvestir.getDateDernierInvest());

            //cas 2 : Investissment dans le futur -> Impossible
            if (dateInvest > dateDuJour)
            {
                throw new Exception("La date de la transaction ne peut pas être dans le futur.");
            }

            double montant = this.calculerInvestissementTotal(transactions);

            //cas 3: Investissement dans le présent
            if (dateInvest == dateDuJour)
            {
                //cas 3.1 : le dernier invest n'était pas le jour précedent cet investissement
                if (dateDernierInvest != dateInvest.AddDays(-1))
                {
                    this.ajouterInvestissementsTotauxManquant(dateDernierInvestNonNull, dateInvest);
                }

                //cas 3.2 : le dernier invest était le jour précedent cet investissement
                _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);
            }

            //cas 4 : On investit dans le passé
            if(dateInvest < dateDuJour)
            {
                this.ajouterInvestissementsTotauxManquant(dateDernierInvestNonNull, dateDuJour);

                //cas 4.1 : la date du premier invest est plus récente que la date d'invest
                if (datePremierInvest > dateInvest)
                {
                    _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);
                    this.changerDatePremierInvest(dateInvest);
                    this.changerAncienPremierInvest(datePremierInvest, montant);
                    this.ajouterInvestissementsTotauxManquant(dateInvest, datePremierInvest);
                    this.modifierInvestissementsTotauxSuivant(datePremierInvest, montant);
                }
                //cas 4.2 : mais la date d'invest est plus récente que le dernier invest
                else if (dateInvest > dateDernierInvestNonNull)
                {
                    _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);
                }
                //cas 4.3 : mais la date d'invest est plus lointaine que le denrier invest
                else if (dateInvest <= dateDernierInvestNonNull)
                {
                    _Iinvestir.modifierInvestissementTotal(dateInvest, montant);
                    this.modifierInvestissementsTotauxSuivant(dateInvest, montant);
                }
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
            DateTime dateCourante = Convert.ToDateTime(dateDebut).AddDays(1); //+1j car on a dejà la date du dernierInvest
            if (dateCourante != dateFin)
            {
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
