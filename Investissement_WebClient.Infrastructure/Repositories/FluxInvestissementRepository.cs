using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Infrastructure.APIs.TradeRepublic;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class FluxInvestissementRepository(IDbContextFactory<InvestissementDbContext> dbContext, 
                                              IOptions<TradeRepublicApiOptions> options) : IFluxInvestissementRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;
        private readonly TradeRepublicApiOptions _options = options.Value;

        public async Task<IEnumerable<FluxInvestissementDto>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var data = await context.FluxInvestissement
                .Include(f => f.Actif)
                .Where(f => f.UtilisateurId == userId)
                .ToListAsync();

            return data
                .Select(t => new FluxInvestissementDto
                {
                    Date = t.Date,
                    Actif = t.Actif!.Libelle,
                    Ticker = t.Actif.Ticker,
                    Logo = ConstruireUrlLogoActifByIsin(t.Actif.ISIN),
                    Prix = t.Prix,
                    Quantite = t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite,
                });
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

            var data = await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new
                {
                    t.Actif!.Libelle,
                    t.Actif.Ticker,
                    t.Actif.ISIN
                })
                .Select(g => new
                {
                    g.Key.Libelle,
                    g.Key.Ticker,
                    g.Key.ISIN,
                    TotalQuantite = g.Sum(t => t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite),
                    TotalValeurInvestie = g.Sum(t => t.Type == TypeFlux.Achat ? t.Quantite * t.Prix : -t.Quantite * t.Prix)
                })
                .ToListAsync();


            return data
                .Select(d => new PositionInvestissementDto
                {
                    Actif = d.Libelle,
                    Ticker = d.Ticker,
                    Logo = ConstruireUrlLogoActifByIsin(d.ISIN),
                    TotalQuantite = d.TotalQuantite,
                    TotalValeurInvestie = d.TotalValeurInvestie
                });
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

        private string ConstruireUrlLogoActifByIsin(string isin)
        {
            return string.Format(_options.LogoBaseUrl, isin);
        }
    }
}
