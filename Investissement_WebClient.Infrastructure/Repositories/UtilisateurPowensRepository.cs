using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class UtilisateurPowensRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IUtilisateurPowensRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<UtilisateurPowens?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.UtilisateurPowens
                .Include(u => u.Banques)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Add(UtilisateurPowens acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.UtilisateurPowens.AddAsync(acces);
            await context.SaveChangesAsync();
        }

        public async Task Update(UtilisateurPowens acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            context.UtilisateurPowens.Update(acces);
            await context.SaveChangesAsync();
        }
    }
}
