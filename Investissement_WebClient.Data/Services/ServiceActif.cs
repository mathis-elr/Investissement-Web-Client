using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;
public class ServiceActif : IServiceActif
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public ServiceActif(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }

    public async Task<IEnumerable<(int Id, string Nom)>> GetActifsDisponibles()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifs = await context.Actifs
            .Select(a => new {a.Id,a.Nom})
            .ToListAsync();
        
        return actifs.Select(a => (a.Id, a.Nom));
    }
    
    public async Task<IEnumerable<(int Id, string Nom)>> GetActifsEnregistres()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var actifs = await context.ActifEnregistres
            .Select(a => new {a.Id, a.Nom})
            .ToListAsync();
        
        return actifs.Select(a => (a.Id, a.Nom));
    }

    public async Task<Actif> GetActifDisponible(int idActif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        return await context.Actifs.Where(a => a.Id == idActif).FirstAsync();
    }

    public async Task<ActifEnregistre> GetActifEnregistre(int idActif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        return await context.ActifEnregistres.Where(a => a.Id == idActif).FirstAsync();
    }

    public async Task SupprimerActifs(List<int> idActifs)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        context.ActifEnregistres.RemoveRange(context.ActifEnregistres.Where(a => idActifs.Contains(a.Id)));
        await context.SaveChangesAsync();
    }

    public async Task AjouterActif(Actif actif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifEnregistre = new ActifEnregistre
        {
            Id = actif.Id,
            Nom =  actif.Nom,
            Type = actif.Type,
            Isin =  actif.Isin,
            Symbole = actif.Symbole,
            Risque = actif.Risque
        };
        context.ActifEnregistres.Add(actifEnregistre);
        
        await context.SaveChangesAsync();
    }
    
    public async Task ModifierActif(ActifEnregistre actif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        context.ActifEnregistres.Update(actif);
        await context.SaveChangesAsync();
    }
}