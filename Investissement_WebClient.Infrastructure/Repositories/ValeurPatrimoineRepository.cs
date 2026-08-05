using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class ValeurPatrimoineRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IValeurPatrimoineRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValuesByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var donneesGroupes = await context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId)
                .GroupBy(hp => hp.Date.Date)
                .Select(d => new
                {
                    Date = d.Key,
                    Max = d.Max(max => max.Valeur - max.InvestissementTotal),
                    Min = d.Min(min => min.Valeur - min.InvestissementTotal),
                    DonneesJour = d
                        .OrderBy(hp => hp.Date)
                        .Select(hp => new
                        {
                            hp.Valeur,
                            hp.InvestissementTotal
                        })
                        .ToList()
                })
                .OrderBy(dg => dg.Date)
                .ToListAsync();

            return donneesGroupes.Select(dg =>
            {
                var premiereDonnee = dg.DonneesJour.FirstOrDefault();
                var derniereDonnee = dg.DonneesJour.LastOrDefault();

                decimal valeurOuverture = premiereDonnee?.Valeur ?? 0;
                decimal valeurFermeture = derniereDonnee?.Valeur ?? 0;
                decimal investissementOuverture = premiereDonnee?.InvestissementTotal ?? 0;
                decimal investissementFermeture = derniereDonnee?.InvestissementTotal ?? 0;

                return new BougieJournaliereCandleChartVM
                {
                    Date = dg.Date,
                    Ouverture = Math.Round(valeurOuverture - investissementOuverture, 2),
                    Fermeture = Math.Round(valeurFermeture - investissementFermeture, 2),
                    Haut = Math.Round(dg.Max, 2),
                    Bas = Math.Round(dg.Min, 2),
                };
            }).ToList();
        }

        public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var data = await context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId)
                .GroupBy(hp => hp.Date.Date)
                .Select(hp => new
                {
                    Date = hp.Key,
                    MaxValeur = hp.Max(t => t.Valeur),
                    MinValeur = hp.Min(t => t.Valeur),
                    DonneesParJour = hp
                        .OrderBy(t => t.Date)
                        .Select(t => new
                        {
                            t.Valeur,
                            t.InvestissementTotal
                        })
                        .ToList(),
                    InvestissementTotal = hp.Max(t => t.InvestissementTotal)
                })
                .OrderBy(hp => hp.Date)
                .ToListAsync();

            return data.Select(t => new BougieJournaliereCandleChartVM
            {
                Date = t.Date,
                Ouverture = t.DonneesParJour.FirstOrDefault()?.Valeur ?? 0,
                Fermeture = t.DonneesParJour.LastOrDefault()?.Valeur ?? 0,
                Bas = t.MinValeur,
                Haut = t.MaxValeur,
                InvestissementTotal = t.InvestissementTotal,
            }).ToList();
        }

        public async Task<DateTime?> GetDateDernierEnregistrement()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.ValeurPatrimoine
                .AsNoTracking()
                .OrderByDescending(h => h.Date)
                .Select(f => (DateTime?)f.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ValeurPatrimoine>> GetHistoriqueAnneeByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var unAnAvantAjd = DateTime.Now.AddYears(-1);

            return await context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId)
                .Where(h => h.Date >= unAnAvantAjd)
                .OrderByDescending(h => h.Date)
                .ToListAsync();
        }

        public async Task<List<PositionPatrimoineUtilisateurDto>> GetPositionsPatrimoineParUtilisateur()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .AsNoTracking()
                .Where(f => f.Actif != null)
                .GroupBy(f => new
                {
                    f.UtilisateurId,
                    f.Actif!.Ticker
                })
                .Select(f => new PositionPatrimoineUtilisateurDto
                {
                    UtilisateurId = f.Key.UtilisateurId,
                    Ticker = f.Key.Ticker,
                    Total = f.Sum(i => i.Type == TypeFlux.Achat ? i.Total : -i.Total),
                    Quantite = f.Sum(i => i.Type == TypeFlux.Achat ? i.Quantite : -i.Quantite)
                })
                .ToListAsync();
        }

        public async Task AddRange(List<ValeurPatrimoine> valeursPatrimoine)
        {
            if (valeursPatrimoine.Count == 0)
                return;

            await using var context = await _dbFactory.CreateDbContextAsync();

            await context.ValeurPatrimoine.AddRangeAsync(valeursPatrimoine);

            await context.SaveChangesAsync();
        }
    }
}