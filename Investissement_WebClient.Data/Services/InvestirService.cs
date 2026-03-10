using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class InvestirService : IInvestirService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public InvestirService(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }

    public async Task<IEnumerable<InvestissementDto>> GetInvestissements()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var investissements = await  context.Investissements
            .Select(i => new InvestissementDto
            {
                Id = i.Id,
                Date = i.DateInvest,
                NomModele = i.IdModele != null ? i.Modele.Nom : null,
                Transactions = i.Transactions.Select(t => new TransactionDto
                {
                    IdActif = t.IdActifEnregistre,
                    NomActif = t.ActifEnregistre.Nom,
                    Quantite = t.Quantite,
                    Prix = t.Prix
                })
            })
            .OrderByDescending(i => i.Date)
            .ToArrayAsync();

        return investissements;
    }

    public async Task SaveInvestissement(int? idModele, DateTime dateInvest,
        List<TransactionDto> transactionsInvestissement)
    {
        if (transactionsInvestissement.Any(t => t.Quantite == null || t.Prix == null || t.Quantite <= 0 || t.Prix <= 0))
        {
            throw new Exception("La quantité et le prix doivent être des valeur valides (supérieures à 0 et champs obligatoires).");
        }


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
            Quantite = pt.Quantite.Value,
            Prix = pt.Prix.Value,
            Frais = null,
            IdActifEnregistre = pt.IdActif,
            IdInvestissement = investissement.Id,
        }).ToList();

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }
}