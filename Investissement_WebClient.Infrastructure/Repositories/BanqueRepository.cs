using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class BanqueRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IBanqueRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<Banque>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Banque
                .Include(b => b.UtilisateurPowens)
                .ToListAsync();
        }

        public async Task<Banque?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Banque
                .Include(b => b.UtilisateurPowens)
                .Include(b => b.Comptes)
                .Where(b => b.UtilisateurPowens.UtilisateurId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Banque>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Banque
                .Include(b => b.UtilisateurPowens)
                .Include(b => b.Comptes)
                .Where(b => b.UtilisateurPowens.UtilisateurId == userId)
                .ToListAsync();
        }

        public async Task Add(Banque acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.Banque.AddAsync(acces);
            await context.SaveChangesAsync();
        }

        public async Task Update(Banque acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            context.Banque.Update(acces);
            await context.SaveChangesAsync();
        }
    }
}
