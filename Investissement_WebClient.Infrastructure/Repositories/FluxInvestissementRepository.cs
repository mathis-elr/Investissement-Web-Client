using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Application.DTO;
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
                    t.Actif.Ticker
                })
                .Select(g => new PositionInvestissementDto
                {
                    Actif = g.Key.Libelle,
                    Ticker = g.Key.Ticker,
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

        public async Task<List<InvestissementParMoisVM>> GetInvestissementParMoisByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var rawData = await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
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
                .Select(d => new InvestissementParMoisVM
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
