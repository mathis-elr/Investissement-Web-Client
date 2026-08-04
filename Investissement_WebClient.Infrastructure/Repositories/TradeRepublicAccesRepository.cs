using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class TradeRepublicAccesRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ITradeRepublicAccesRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<TradeRepublicAcces?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.TradeRepublicAcces
                .Where(b => b.UtilisateurId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Add(TradeRepublicAcces acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.TradeRepublicAcces.AddAsync(acces);
            await context.SaveChangesAsync();
        }
    }
}
