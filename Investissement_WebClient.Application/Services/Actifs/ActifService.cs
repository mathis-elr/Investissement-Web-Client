using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.Actifs
{
    public class ActifService : IActifService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

        public ActifService(IDbContextFactory<InvestissementDbContext> dbContext)
        {
            _dbFactory = dbContext;
        }

        public async Task<List<Actif>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.Actif.ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTickers()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Actif.Select(d => d.Ticker).ToListAsync();
        }

        public async Task<int> AddActif(Actif actif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.AddAsync(actif);
            await context.SaveChangesAsync();
            return actif.Id;
        }
    }
}
