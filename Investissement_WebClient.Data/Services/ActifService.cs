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
        
        return await context.Actifs.Where(a => !a.EstEnregistre)
            .Select(a => new ActifDto
            {
                Id = a.Id,
                Nom = a.Nom,
                Type = a.Type,
                Isin = a.Isin,
                Symbole = a.Symbole,
                Risque = a.Risque.Value
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ActifDto>> GetActifsEnregistres()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        return await context.Actifs.Where(a => a.EstEnregistre)
            .Select(a => new ActifDto
            {
                Id = a.Id,
                Nom = a.Nom,
                Type = a.Type,
                Isin = a.Isin,
                Symbole = a.Symbole,
                Risque = a.Risque.Value
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<DetailsActifDto>> GetDetailsActif()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        IEnumerable<DetailsActifDto> detailsActifDtos = context.Transactions.GroupBy(t => t.IdActifEnregistre)
            .Select(a => new DetailsActifDto
            {
                NomActif = a.First().Actif.Nom,
                SymboleActif = a.First().Actif.Symbole,
                QuantiteDetenue = a.Sum(t => t.Quantite),
            }).ToList();

        return detailsActifDtos;
    }

    public async Task EnregistrerActif(ActifDto actif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifEnregistre = new Actif
        {
            Id = actif.Id,
            Nom =  actif.Nom,
            Type = actif.Type,
            Isin =  actif.Isin,
            Symbole = actif.Symbole,
            Risque = actif.Risque,
            EstEnregistre = true
        };
        
        context.Actifs.Update(actifEnregistre);
        await context.SaveChangesAsync();
    }
    
    public async Task ModifierActif(ActifDto actif)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var actifUpdate = new Actif
        {
            Id = actif.Id,
            Nom =  actif.Nom,
            Type = actif.Type,
            Isin =  actif.Isin,
            Symbole = actif.Symbole,
            Risque = actif.Risque,
            EstEnregistre = true
        };
        
        context.Actifs.Update(actifUpdate);
        await context.SaveChangesAsync();
    }
    
    public async Task SupprimerActifs(List<ActifDto> actifs)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var actifsNonEnregistre = actifs.Select(a => new Actif
        {
            Id = a.Id,
            Nom = a.Nom,
            Type = a.Type,
            Isin = a.Isin,
            Symbole = a.Symbole,
            Risque = a.Risque,
            EstEnregistre = false
        });
        
        context.Actifs.UpdateRange(actifsNonEnregistre);
        await context.SaveChangesAsync();
    }
}