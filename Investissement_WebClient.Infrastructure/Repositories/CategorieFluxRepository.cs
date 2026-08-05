using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class CategorieFluxRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ICategorieFluxRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<List<CategorieFlux>> GetAll()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return context.CategorieFlux.ToList();
        }
    }
}
