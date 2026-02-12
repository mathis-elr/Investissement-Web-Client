using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class TransactionService : ITransactionService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public TransactionService(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }

    public async Task<IEnumerable<DetailsActifDto>> GetDetailsActif()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        IEnumerable<DetailsActifDto> detailsActifDtos = context.Transactions.GroupBy(t => t.IdActifEnregistre)
            .Select(a => new DetailsActifDto
        {
            NomActif = a.First().ActifEnregistre.Nom,
            SymboleActif = a.First().ActifEnregistre.Symbole,
            QuantiteDetenue = a.Sum(t => t.Quantite),
        }).ToList();
        
        return detailsActifDtos;
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
}