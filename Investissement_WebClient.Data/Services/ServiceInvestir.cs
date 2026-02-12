using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
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
    
    public async Task<IEnumerable<ItemDto>> GetModeles()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var modele = await context.Modeles
            .Select(m => new ItemDto
            {
                Id = m.Id,
                Nom = m.Nom
            })
            .ToListAsync();
        
        return modele;
    }

    public async Task<List<TransactionDto>> GetCompositionModele(int idModele)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var compositionModele = await context.CompositionModeles
            .Where(cm => cm.IdModele == idModele)
            .Select(cm => new TransactionDto()
                {
                    IdActif = cm.IdActifEnregistre, 
                    NomActif = cm.ActifEnregistre.Nom, 
                    Quantite = cm.Quantite, 
                })
            .ToListAsync();
        
        return compositionModele;
    }

    public async Task SaveInvestissement(int? idModele, DateTime dateInvest,
        List<TransactionDto> transactionsInvestissement)
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

    public async Task AjouterModele(string nomModele, List<TransactionDto> compositionModele)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var modele = new Modele
        {
            Nom = nomModele
        };
        
        context.Modeles.Add(modele);
        await context.SaveChangesAsync();

        var composition = compositionModele.Select(pt => new CompositionModele
        {
            IdActifEnregistre = pt.IdActif,
            IdModele = modele.Id,
            Quantite = pt.Quantite,
        });
        
        await context.CompositionModeles.AddRangeAsync(composition);
        await context.SaveChangesAsync();
    }

    public async Task UpdateModele(ItemDto item, List<TransactionDto> compositionModele)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var updateModele = new Modele
        {
            Id = item.Id,
            Nom = item.Nom,
        };
        
        context.Modeles.Update(updateModele);
        
        var ancienneComposition = context.CompositionModeles
            .Where(c => c.IdModele == item.Id);
        context.CompositionModeles.RemoveRange(ancienneComposition);
        
        await context.SaveChangesAsync();

        var nouvelleComposition = compositionModele.Select(pt => new CompositionModele
        {
            IdActifEnregistre = pt.IdActif,
            IdModele = item.Id,
            Quantite = pt.Quantite,
        });
        
        await context.CompositionModeles.AddRangeAsync(nouvelleComposition);
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteModeles(List<int> modeles)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        context.Modeles.RemoveRange(context.Modeles.Where(m => modeles.Contains(m.Id)));
        await context.SaveChangesAsync();
    }
}