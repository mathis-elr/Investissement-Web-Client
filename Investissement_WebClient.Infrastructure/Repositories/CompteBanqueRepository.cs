using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class CompteBanqueRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ICompteBanqueRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<CompteBanque>> GetAllNonInvestissement()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Where(c => c.TypeCompte != TypeCompte.Investissement)
                .Include(c => c.Banque)
                    .ThenInclude(b => b.UtilisateurPowens)
                .ToListAsync();
        }

        public async Task<IEnumerable<CompteBanque>> GetAllByBanqueId(int banqueId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Include(b => b.Banque)
                .Where(b => b.Banque.Id == banqueId)
                .ToListAsync();
        }

        public async Task<List<CompteBanque>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Include(c => c.Banque)
                    .ThenInclude(b => b.UtilisateurPowens)
                .Where(c => c.Banque.UtilisateurPowens.UtilisateurId == userId)
                .ToListAsync();
        }

        public async Task<CompteBanque?> GetById(int id)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Include(c => c.Banque)
                    .ThenInclude(b => b.UtilisateurPowens)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task Add(CompteBanque compte)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.CompteBanque.AddAsync(compte);
            await context.SaveChangesAsync();
        }

        public async Task Update(CompteBanque compte)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            context.CompteBanque.Update(compte);
            await context.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.SaveChangesAsync();
        }
    }
}
