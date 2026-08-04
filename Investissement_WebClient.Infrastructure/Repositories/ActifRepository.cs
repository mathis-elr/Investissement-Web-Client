using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class ActifRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IActifRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<List<Actif>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Actif.ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllTickers()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Actif.Select(d => d.Ticker).ToListAsync();
        }

        public async Task<int> Add(Actif actif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.AddAsync(actif);
            await context.SaveChangesAsync();
            return actif.Id;
        }
    }
}
