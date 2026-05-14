using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.Actifs
{
    public class ActifService : IActifService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

        private readonly List<string> _motsInutiles = ["EUR", "(ACC)", "PEA", "SWAP", "(DIST)","ESG"];

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

        public string NettoyerLibelle(string libelle)
        {
            if (string.IsNullOrWhiteSpace(libelle))
                return string.Empty;

            var motsNettoyes = libelle.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(mot => !_motsInutiles.Contains(mot, StringComparer.OrdinalIgnoreCase));

            string resultat = string.Join(" ", motsNettoyes);

            return resultat.Trim();
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
