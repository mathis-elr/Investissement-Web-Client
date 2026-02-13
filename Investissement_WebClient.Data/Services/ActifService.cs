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

    public async Task<IEnumerable<ActifDto>> GetActifsDisponibles()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        return await context.Actifs
            .Select(a => new ActifDto
            {
                Id = a.Id,
                Nom = a.Nom,
                Type = a.Type.Value,
                Isin = a.Isin,
                Symbole = a.Symbole,
                Risque = a.Risque.Value
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ActifDto>> GetActifsEnregistres()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        return await context.ActifEnregistres
            .Select(a => new ActifDto
            {
                Id = a.Id,
                Nom = a.Nom,
                Type = a.Type.Value,
                Isin = a.Isin,
                Symbole = a.Symbole,
                Risque = a.Risque.Value
            })
            .ToListAsync();
    }
    
    public ActifTypesDto GetActifsParType(IEnumerable<ActifDto> actifs)
    {
        return new ActifTypesDto
        {
            Etfs = actifs.Where(a => a.Type == ActifType.ETF),
            Etcs = actifs.Where(a => a.Type == ActifType.ETC),
            Cryptos = actifs.Where(a => a.Type == ActifType.Crypto),
            Actions =  actifs.Where(a => a.Type == ActifType.Action),
            Obligations =  actifs.Where(a => a.Type == ActifType.Obligation),
        };
    }

    public async Task SupprimerActifs(List<int> idActifs)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        context.ActifEnregistres.RemoveRange(context.ActifEnregistres.Where(a => idActifs.Contains(a.Id)));
        await context.SaveChangesAsync();
    }

    public async Task AjouterActif(ActifDto actif)
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
    
    public async Task ModifierActif(ActifDto actif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifUpdate = new ActifEnregistre
        {
            Id = actif.Id,
            Nom =  actif.Nom,
            Type = actif.Type,
            Isin =  actif.Isin,
            Symbole = actif.Symbole,
            Risque = actif.Risque
        };
        
        context.ActifEnregistres.Update(actifUpdate);
        await context.SaveChangesAsync();
    }
}