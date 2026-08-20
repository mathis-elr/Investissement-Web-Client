using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class CompteBanqueRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ICompteBanqueRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<IEnumerable<CompteBanque>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
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

        public async Task<List<CompteBanqueDto>> GetAllByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Include(c => c.Banque)
                    .ThenInclude(b => b.UtilisateurPowens)
                .Where(c => c.Banque.UtilisateurPowens.UtilisateurId == userId)
                .Select(c => new CompteBanqueDto
                {
                    Id = c.Id,
                    IdComptePowens = c.IdComptePowens,
                    Nom = c.Nom,
                    TypePowens = c.TypePowens,
                    Banque = c.Banque
                })
                .ToListAsync();
        }

        public async Task<CompteBanque?> GetByBanqueId(int banqueId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.CompteBanque
                .Include(b => b.Banque)
                .Where(b => b.Banque.Id == banqueId)
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
    }
}
