using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class FluxInvestissementRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IFluxInvestissementRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<FluxInvestissementDto>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
            .Include(f => f.Actif)
            .Where(f => f.UtilisateurId == userId)
            .Select(t => new FluxInvestissementDto
            {
                Date = t.Date,
                Actif = t.Actif!.Libelle,
                Ticker = t.Actif.Ticker,
                Logo = t.Actif.Logo != null ? $"data:image/png;base64,{Convert.ToBase64String(t.Actif.Logo)}" : null,
                Prix = t.Prix,
                Quantite = t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite,
            }).ToListAsync();
        }

        public async Task<FluxInvestissement?> GetLastByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .OrderByDescending(f => f.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<FluxInvestissement?> GetFirstDateByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .OrderBy(f => f.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PositionActifDto>> GetPositionsParActifByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new
                {
                    t.Actif!.Libelle,
                    t.Actif.Ticker
                })
                .Select(g => new PositionActifDto
                {
                    Actif = g.Key.Libelle,
                    Ticker = g.Key.Ticker,
                    QuantiteTotale = g.Sum(t => t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite)
                })
                .ToListAsync();
        }

        public async Task<decimal> GetValeurInvestissementTotalByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .SumAsync(t => t.Type == TypeFlux.Achat ? t.Total : -t.Total);
        }

        public async Task<IEnumerable<PositionInvestissementDto>> GetPositionsInvestiesParActifByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new
                {
                    t.Actif!.Libelle,
                    t.Actif.Ticker,
                    t.Actif.Logo
                })
                .Select(g => new PositionInvestissementDto
                {
                    Actif = g.Key.Libelle,
                    Ticker = g.Key.Ticker,
                    Logo = g.Key.Logo != null ? $"data:image/png;base64,{Convert.ToBase64String(g.Key.Logo)}" : null,
                    TotalQuantite = g.Sum(t =>
                        t.Type == TypeFlux.Achat
                            ? t.Quantite
                            : -t.Quantite),

                    TotalValeurInvestie = g.Sum(t =>
                        t.Type == TypeFlux.Achat
                            ? t.Quantite * t.Prix
                            : -t.Quantite * t.Prix)
                })
                .ToListAsync();
        }

        public async Task<List<InvestissementParMoisDto>> GetInvestissementParMoisByUserId(PeriodeHistoriqueInvest periode, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var query = context.FluxInvestissement
            .AsNoTracking()
            .Where(h => h.UtilisateurId == userId);

            if (periode != PeriodeHistoriqueInvest.Tout)
            {
                var dateLimite = DateTime.UtcNow.AddDays(-((int)periode));
                query = query.Where(h => h.Date >= dateLimite);
            }

            var rawData = await query
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(d => new
                {
                    Annee = d.Key.Year,
                    Mois = d.Key.Month,
                    TotalInvestit = d.Sum(t =>
                        t.Type == TypeFlux.Achat
                            ? t.Total
                            : -t.Total)
                })
                .ToListAsync();

            return rawData
                .Select(d => new InvestissementParMoisDto
                {
                    Date = new DateTime(d.Annee, d.Mois, 1),
                    Investissement = Math.Round(d.TotalInvestit, 2)
                })
                .OrderByDescending(d => d.Date)
                .ToList();
        }

        public async Task AddRange(List<FluxInvestissement> flux)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            await context.FluxInvestissement.AddRangeAsync(flux);
            await context.SaveChangesAsync();
        }
    }
}
