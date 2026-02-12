using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;
public class ActifService : IActifService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public ActifService(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }

    public async Task<IEnumerable<ItemDto>> GetActifsDisponibles()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifs = await context.Actifs
            .Select(a => new ItemDto
            {
                Id = a.Id,
                Nom = a.Nom,
            })
            .ToListAsync();
        
        return actifs;
    }
    
    public async Task<IEnumerable<ItemDto>> GetActifsEnregistres()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var actifs = await context.ActifEnregistres
            .Select(a => new ItemDto
            {
                Id = a.Id,
                Nom = a.Nom,
            })
            .ToListAsync();
        
        return actifs;
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