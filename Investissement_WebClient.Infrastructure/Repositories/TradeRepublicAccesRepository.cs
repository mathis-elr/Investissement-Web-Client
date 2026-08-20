using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class TradeRepublicAccesRepository(IDbContextFactory<InvestissementDbContext> dbContext) : ITradeRepublicAccesRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<SourceDto?> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.TradeRepublicAcces
                .Where(b => b.Utilisateur.Id == userId)
                .Select(c => new SourceDto
                {
                    Id = -1,
                    NomCompte = "Investissement",
                    NomSource = "Trade Républic"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CompteTradeRepublic?> GetLoginByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.TradeRepublicAcces
                .Where(b => b.Utilisateur.Id == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Add(CompteTradeRepublic acces)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            await context.TradeRepublicAcces.AddAsync(acces);
            await context.SaveChangesAsync();
        }
    }
}
