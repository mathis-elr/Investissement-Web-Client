using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class PositionInvestissementRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IPositionInvestissementRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<List<PositionInvestissement>> GetByCompteBanqueId(int compteId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.PositionInvestissement
                .Where(f => f.CompteBanqueId == compteId)
                .Include(p => p.Actif)
                .Include(p => p.CompteBanque)
                .ToListAsync();
        }

        public async Task AddRange(List<PositionInvestissement> positions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.AddRangeAsync(positions);
            await context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.SaveChangesAsync();
        }
    }
}
