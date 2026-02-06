using Investissement_WebClient.Core.InterfacesServices;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class ServiceActif : IServiceActif
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public ServiceActif(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }
    
    public async Task<IEnumerable<(int Id, string Nom)>> GetNomActifsEnregistres()
    {
        using var context = _dbFactory.CreateDbContext();

        var actifs = await context.ActifEnregistres
            .Select(actif => new {actif.Id, actif.Nom})
            .ToListAsync();
        
        return actifs.Select(a => (a.Id, a.Nom));
    }
}