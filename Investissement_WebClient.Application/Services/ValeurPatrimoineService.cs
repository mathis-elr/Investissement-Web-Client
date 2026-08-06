using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Services
{
    public class ValeurPatrimoineService(IValeurPatrimoineRepository valeurPatrimoineRepository) : IValeurPatrimoineService
    {
        private readonly IValeurPatrimoineRepository _valeurPatrimoineRepository = valeurPatrimoineRepository;

        public async Task<IEnumerable<BougieChartDto>> GetBougiesPlusValueByUserId(Periode periode, Granulometrie granulometrie, int userId)
        {
            return await _valeurPatrimoineRepository.GetBougiesPlusValueByUserId(periode, granulometrie, userId);
        }

        public async Task<IEnumerable<PointChartDto>> GetPointsPlusValueByUserId(Periode periode, Granulometrie granulometrie, int userId)
        {
            return await _valeurPatrimoineRepository.GetPointsPlusValueByUserId(periode, granulometrie, userId);
        }

        public async Task<IEnumerable<BougieChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId)
        {
            return await _valeurPatrimoineRepository.GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(userId);
        }

        public async Task<DateTime?> GetDateDernierEnregistrement()
        {
            return await _valeurPatrimoineRepository.GetDateDernierEnregistrement();
        }

        public async Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId)
        {
            var historique = await _valeurPatrimoineRepository.GetAllHistoriqueByUserId(userId);

            return new List<VariationDto>
            {
                new() { Label = "24H", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historique, 1) },
                new() { Label = "7J", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historique, 7) },
                new() { Label = "1M", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historique, 30) },
                new() { Label = "1A", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historique, 365) },
                new() { Label = "All", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historique, 0) },
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
            if (valeurInvestissementTotal == 0 || historique.Count == 0)
                return 0;

            decimal ancienProfit;
            if (periode == 0)
            {
                ancienProfit = historique
                    .OrderBy(h => h.Date)
                    .Select(h => h.Valeur - h.InvestissementTotal)
                    .FirstOrDefault();
            }
            else
            {
                DateTime dateDebutPeriode = DateTime.Now.AddDays(-periode);

                var historiquePeriode = historique
                    .Where(h => h.Date >= dateDebutPeriode)
                    .OrderBy(h => h.Date)
                    .ToList();

                if (historiquePeriode.Count == 0)
                    return 0;

                ancienProfit = historiquePeriode
                    .Select(h => h.Valeur - h.InvestissementTotal)
                    .First();
            }

            decimal nouveauProfit = valeurActuelle - valeurInvestissementTotal;

            return (nouveauProfit - ancienProfit) / valeurInvestissementTotal;
        }
    }
}