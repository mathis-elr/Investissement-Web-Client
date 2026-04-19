using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class FluxCreditCoopService : IFluxCreditCoopService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public FluxCreditCoopService(IDbContextFactory<InvestissementDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task AddFluxCreditCoop(List<FluxCreditCoopDto>? fluxCreditCoop)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        if (fluxCreditCoop == null || fluxCreditCoop.Count == 0)
        {
            Console.WriteLine("Aucun flux à récuperer pour la periode donnée.");
            return;
        }

        var idsExistants = await context.FluxCreditCoop
            .Select(f => f.Id)
            .ToListAsync();

        var nvFlux = fluxCreditCoop
            .Where(f => idsExistants.Contains(f.Id))
            .Select(f => new FluxCreditCoop
            {
                Id = f.Id,
                Date = f.Date,
                Valeur = f.Valeur,
                LibelleRecu = f.LibelleRecu
            });

        context.FluxCreditCoop.AddRange(nvFlux);
        await context.SaveChangesAsync();
    }
}