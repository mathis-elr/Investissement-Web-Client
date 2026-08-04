using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class UtilisateurRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IUtilisateurRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<Utilisateur?> GetByEmail(string email)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Utilisateur.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> Add(Utilisateur utilisateur)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            dbContext.Utilisateur.Add(utilisateur);
            await dbContext.SaveChangesAsync();
            return utilisateur.Id;
        }
    }
}
