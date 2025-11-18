using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Metadata;
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

            this.ajouterInvestissementTotal(transactions);
        }

        private void ajouterInvestissementTotal(List<Transaction> transactions)
        {
            DateTime dateInvest = transactions[0].date.Date;
            DateTime dateDuJour = DateTime.Today;
            //cas 1 : Investissment dans le futur -> Impossible
            if (dateInvest > dateDuJour)
            {
                throw new Exception("La date de la transaction ne peut pas être dans le futur.");
            }

            DateTime? dateDernierInvestNullable = _Iinvestir.getDateDernierInvest();
            //cas 1 : On investit pour la première fois
            if (dateDernierInvestNullable == null)
            {
                _Iinvestir.ajouterInvestissementTotal(dateInvest.AddDays(-1), 0); //on ajoute au jour precedent le jour de l'investissement
            }

            //on rajoute les investissements manquants, du dernier invest+1 jusqu'a ajd inclu
            DateTime dateDernierInvestNonNull = Convert.ToDateTime(_Iinvestir.getDateDernierInvest());
            this.ajouterInvestissementsTotauxManquant(dateDernierInvestNonNull, dateDuJour);

            double montant = this.calculerInvestissementTotal(transactions);

            //cas 3: Investissement dans le présent
            if (dateInvest == dateDuJour)
            {
                _Iinvestir.modifierInvestissementTotal(dateInvest, montant);
            }

            DateTime datePremierInvest = _Iinvestir.getDatePremierInvest();

            //cas 4 : On investit dans le passé..
            if (dateInvest < dateDuJour)
            {
                //cas 4.1 : et la date du premier invest est plus récente que la date d'invest
                if (dateInvest < datePremierInvest)
                {
                    this.changerDatePremierInvest(dateInvest);
                    this.changerAncienPremierInvest(datePremierInvest, montant);

                    _Iinvestir.ajouterInvestissementTotal(dateInvest, montant);
                    this.ajouterInvestissementsTotauxManquant(dateInvest, datePremierInvest.AddDays(-1));
                    this.modifierInvestissementsTotauxSuivant(datePremierInvest, montant);
                }
                //cas 4.2 : et la date d'invest est plus récente que le dernier invest
                else if (dateInvest > datePremierInvest)
                {
                    _Iinvestir.modifierInvestissementTotal(dateInvest, montant);
                    this.modifierInvestissementsTotauxSuivant(dateInvest, montant);
                }
                ////cas 4.3 : et la date d'investissement correspond au premier investissement (celui qui vaut 0)
                else if (dateInvest == datePremierInvest)
                {
                    this.changerDatePremierInvest(dateInvest);
                    _Iinvestir.modifierInvestissementTotal(dateInvest, montant);
                    this.modifierInvestissementsTotauxSuivant(dateInvest, montant);
                }
            }
        }

        private double calculerInvestissementTotal(List<Transaction> transactions)
        {
            return Convert.ToDouble(transactions.Sum(q => q.quantite * q.prix));
        }

        private void ajouterInvestissementsTotauxManquant(DateTime dateDebut, DateTime dateFin)
        {
            DateTime dateCourante = Convert.ToDateTime(dateDebut).AddDays(1);

            while (dateCourante <= dateFin)
            {
                _Iinvestir.ajouterInvestissementTotal(dateCourante, 0);
                dateCourante = dateCourante.AddDays(1);
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

            while (dateCourante <= dateDernierInvest)
            {
                Console.WriteLine(dateCourante);
                _Iinvestir.modifierInvestissementTotal(dateCourante, montant);
                dateCourante = dateCourante.AddDays(1);
            }
        }
    }
}
