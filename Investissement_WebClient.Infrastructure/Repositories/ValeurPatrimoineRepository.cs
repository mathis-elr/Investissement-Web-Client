using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using Microsoft.EntityFrameworkCore;


namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class ValeurPatrimoineRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IValeurPatrimoineRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<BougieChartDto>> GetBougiesPlusValueByUserId(Periode periode, Granulometrie granulometrie, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var query = context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId);

            if(periode != Periode.Tout)
            {
                var dateLimite = DateTime.UtcNow.AddDays(-((int)periode));
                query = query.Where(h => h.Date >= dateLimite);
            }

            var donnees = await query
                .Select(d => new
                {
                    d.Date,
                    PlusValue =   d.Valeur - d.InvestissementTotal
                })
                .OrderBy(dg => dg.Date)
                .ToListAsync();

            var donneesGroupes = donnees
                .GroupBy(d => GetDateGroupe(d.Date, granulometrie))
                .OrderBy(dg => dg.Key)
                .ToList();

            return donneesGroupes
                .Select(dg =>
                {
                    var points = dg
                    .OrderBy(d => d.Date)
                    .ToList();

                    return new BougieChartDto
                    {
                        Date = dg.Key,
                        Ouverture = Math.Round(points.First().PlusValue, 2),
                        Fermeture = Math.Round(points.Last().PlusValue, 2),
                        Haut = Math.Round(points.Max(p => p.PlusValue), 2),
                        Bas = Math.Round(points.Min(p => p.PlusValue), 2),
                    };
                }).ToList();
        }


        public async Task<IEnumerable<PointChartDto>> GetPointsPlusValueByUserId(Periode periode, Granulometrie granulometrie, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var query = context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId);

            if (periode != Periode.Tout)
            {
                var dateLimite = DateTime.UtcNow.AddDays(-((int)periode));
                query = query.Where(h => h.Date >= dateLimite);
            }

            var donnees = await query
                .Select(d => new
                {
                    d.Date,
                    PlusValue = d.Valeur - d.InvestissementTotal
                })
                .OrderBy(dg => dg.Date)
                .ToListAsync();

            var donneesGroupes = donnees
                .GroupBy(d => GetDateGroupe(d.Date, granulometrie))
                .OrderBy(dg => dg.Key)
                .ToList();

            return donneesGroupes
                .Select(dg =>
                {
                    var points = dg
                    .OrderBy(d => d.Date)
                    .ToList();

                    return new PointChartDto
                    {
                        Date = dg.Key,
                        Valeur = Math.Round(points.Average(p => p.PlusValue), 2)
                    };
                }).ToList();
        }

        public async Task<IEnumerable<BougieChartDto>> GetBougiesJournalieresValeurPatrimoineSurInvestissementTotalByUserId(int userId)
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

            return data.Select(t => new BougieChartDto
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

        public async Task<List<ValeurPatrimoine>> GetAllHistoriqueByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.ValeurPatrimoine
                .AsNoTracking()
                .Where(h => h.UtilisateurId == userId)
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

        private static DateTime GetDateGroupe(DateTime date, Granulometrie granulometrie)
        {
            return granulometrie switch
            {
                Granulometrie.Journalier => date.Date,

                Granulometrie.Hebdomadaire => GetDebutSemaine(date),

                Granulometrie.Mensuel => new DateTime(date.Year, date.Month, 1),

                _ => date.Date
            };
        }

        private static DateTime GetDebutSemaine(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.Date.AddDays(-diff);
        }
    }
}