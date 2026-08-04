using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services.ValeurPatrimoines
{
    public class ValeurPatrimoineService(IValeurPatrimoineRepository valeurPatrimoineRepository) : IValeurPatrimoineService
    {
        private readonly IValeurPatrimoineRepository _valeurPatrimoineRepository = valeurPatrimoineRepository;

        public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValues(int userId)
        {
            return await _valeurPatrimoineRepository.GetBougiesJournalieresPlusOuMoinsValuesByUserId(userId);
        }

        public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId)
        {
            return await _valeurPatrimoineRepository.GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(userId);
        }

        public async Task<DateTime?> GetDateDernierEnregistrement()
        {
            return await _valeurPatrimoineRepository.GetDateDernierEnregistrement();
        }

        public async Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId)
        {
            var historiqueAnnee = await _valeurPatrimoineRepository.GetHistoriqueAnneeByUserId(userId);

            return new List<VariationDto>
            {
                new() { Label = "24H", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 1) },
                new() { Label = "7J", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 7) },
                new() { Label = "1M", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 30) },
                new() { Label = "1A", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 365) }
            };
        }

        public async Task SaveValeurPatrimoine(Dictionary<string, decimal> prixParActif)
        {
            var quantiteParActifParUtilisateur = await _valeurPatrimoineRepository.GetPositionsPatrimoineParUtilisateur();

            var now = DateTime.Now;

            var nouvellesValeursPatrimoine = quantiteParActifParUtilisateur
                .GroupBy(q => q.UtilisateurId)
                .Where(q => q.Sum(x => x.Total) > 0)
                .Select(q => new ValeurPatrimoine
                {
                    Date = now,
                    UtilisateurId = q.Key,
                    InvestissementTotal = q.Sum(x => x.Total),
                    Valeur = q.Sum(x =>
                        x.Quantite *
                        (prixParActif.TryGetValue(x.Ticker, out var prix) ? prix : 0))
                })
                .ToList();

            await _valeurPatrimoineRepository.AddRange(nouvellesValeursPatrimoine);
        }

        private decimal CalculVariationPeriode(decimal valeurActuelle, decimal valeurInvestissementTotal, List<ValeurPatrimoine> historique, int periode)
        {
            if (valeurInvestissementTotal == 0)
                return 0;

            DateTime dateDebutPeriode = DateTime.Now.AddDays(-periode);

            var ancienProfit = historique
                .Where(h => h.Date >= dateDebutPeriode)
                .OrderBy(h => h.Date)
                .Select(h => h.Valeur - h.InvestissementTotal)
                .FirstOrDefault();

            decimal nouveauProfit = valeurActuelle - valeurInvestissementTotal;

            return (nouveauProfit - ancienProfit) / valeurInvestissementTotal;
        }
    }
}