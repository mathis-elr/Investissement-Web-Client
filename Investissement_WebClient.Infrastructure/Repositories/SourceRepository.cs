using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class SourceRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ISourceRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<List<Source>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Source
                .Where(s => s.UtilisateurId == userId)
                .ToListAsync();
        }

        public async Task<int> Add(Source source)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.AddAsync(source);
            await context.SaveChangesAsync();
            return source.Id;
        }

        public async Task Update(Source source)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            context.Update(source);
            await context.SaveChangesAsync();
        }
    }
}
