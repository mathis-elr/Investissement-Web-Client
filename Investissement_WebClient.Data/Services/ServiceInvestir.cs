using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class ServiceInvestir : IServiceInvestir
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public ServiceInvestir(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }
    
    public async Task<IEnumerable<(int Id,string Nom)>> GetModeles()
    {
        using var context = _dbFactory.CreateDbContext();
        
        var modele = await context.Modeles
            .Select(m => new { m.Id, m.Nom })
            .ToListAsync();
        
        return modele.Select(m => (m.Id, m.Nom));
    }

    public async Task<IEnumerable<PreparationTransaction>> GetCompositionModele(int idModele)
    {
        using var context = _dbFactory.CreateDbContext();

        var compositionModele = await context.CompositionModeles
            .Where(cm => cm.IdModele == idModele)
            .Select(cm => new PreparationTransaction(cm.IdActifEnregistre, cm.ActifEnregistre.Nom, cm.Quantite, null))
            .ToListAsync();
        
        return compositionModele;
    }
    

    // public async Task<> GetDernierInvest()
    // {
    //     using var context = _dbContext.CreateDbContext();
    // }
}