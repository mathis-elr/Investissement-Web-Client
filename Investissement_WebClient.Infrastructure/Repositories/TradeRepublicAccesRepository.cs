using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class TradeRepublicAccesRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ITradeRepublicAccesRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<CompteTradeRepublic?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteTradeRepublic
                .Where(b => b.Utilisateur.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<CompteTradeRepublic?> GetLoginByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteTradeRepublic
                .Where(b => b.Utilisateur.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Add(CompteTradeRepublic acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.CompteTradeRepublic.AddAsync(acces);
            await context.SaveChangesAsync();
        }
    }
}
