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

    public async Task<IEnumerable<InvestissementGetDto>> GetInvestissements()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var investissements = await  context.Investissements
            .Select(i => new InvestissementGetDto
            {
                Id = i.Id,
                DateInvest = i.DateInvest,
                Modele = i.IdModele != null ? new ModeleDto
                {
                    Id = i.Modele.Id,
                    Nom = i.Modele.Nom,
                } : null,
                Note = i.Note,
                Transactions = i.Transactions.Select(t => new TransactionDto
                {
                    IdActif = t.IdActifEnregistre,
                    NomActif = t.ActifEnregistre.Nom,
                    Quantite = t.Quantite,
                    Prix = t.Prix
                })
            })
            .OrderByDescending(i => i.DateInvest)
            .ToArrayAsync();

        return investissements;
    }

    public async Task SaveInvestissement(InvestissementDto investissementDto)
    {
        


        await using var context = await _dbFactory.CreateDbContextAsync();
        Investissement investissement = new()
        {
            DateInvest = investissementDto.Date,
            IdModele = investissementDto.idModele,
            Note = investissementDto.Note,
        };

        context.Investissements.Add(investissement);
        await context.SaveChangesAsync();

        var transactions = investissementDto.Transactions.Select(pt => new Transaction
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

    public async Task DeleteDernierInvest(InvestissementGetDto investissementGetDto)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        if (context.Investissements.Any(i => i.Id == investissementGetDto.Id))
        {
            Investissement investissement = new Investissement
            {
                Id = investissementGetDto.Id,
                DateInvest = investissementGetDto.DateInvest,
                IdModele = investissementGetDto.Modele?.Id,
                Note = investissementGetDto.Note,
            };
            
            context.Investissements.Remove(investissement);
            await context.SaveChangesAsync();
        }
    }
}