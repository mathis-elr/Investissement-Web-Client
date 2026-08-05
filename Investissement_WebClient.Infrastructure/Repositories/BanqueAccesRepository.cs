using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class BanqueAccesRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IBanqueAccesRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<BanqueAcces?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.BanqueAcces
                .Where(b => b.UtilisateurId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<int>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.BanqueAcces
                .Where(b => b.DateExpiration > DateTime.Now)
                .Select(b => b.UtilisateurId)
                .ToListAsync();
        }

        public async Task Add(BanqueAcces acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.BanqueAcces.AddAsync(acces);
            await context.SaveChangesAsync();
        }

        public async Task Update(BanqueAcces acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            context.BanqueAcces.Update(acces);
            await context.SaveChangesAsync();
        }
    }
}
