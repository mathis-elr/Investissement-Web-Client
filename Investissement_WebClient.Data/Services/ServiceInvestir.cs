using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles;
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
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var modele = await context.Modeles
            .Select(m => new { m.Id, m.Nom })
            .ToListAsync();
        
        return modele.Select(m => (m.Id, m.Nom));
    }

    public async Task<IEnumerable<PreparationTransaction>> GetCompositionModele(int idModele)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var compositionModele = await context.CompositionModeles
            .Where(cm => cm.IdModele == idModele)
            .Select(cm => new PreparationTransaction
                {
                    IdActif = cm.IdActifEnregistre, 
                    NomActif = cm.ActifEnregistre.Nom, 
                    Quantite = cm.Quantite, 
                    Prix = null
                })
            .ToListAsync();
        
        return compositionModele;
    }

    public async Task SaveInvestissement(int? idModele, DateTime dateInvest,
        List<PreparationTransaction> transactionsInvestissement)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        Investissement investissement = new()
        {
            DateInvest = dateInvest,
            IdModele = idModele
        };
        
        context.Investissements.Add(investissement);
        await context.SaveChangesAsync();

        var transactions = transactionsInvestissement.Select(pt => new Transaction
        {
            Quantite = pt.Quantite ?? 0,
            Prix = pt.Prix ?? 0,
            Frais = null,
            IdActifEnregistre = pt.IdActif,
            IdInvestissement = investissement.Id,
        }).ToList();
        
        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }
}