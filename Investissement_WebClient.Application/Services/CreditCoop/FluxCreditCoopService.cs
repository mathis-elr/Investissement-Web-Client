using Investissement_WebClient.Application.ApiResponse;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.CreditCoop;

public class FluxCreditCoopService : IFluxCreditCoopService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public FluxCreditCoopService(IDbContextFactory<InvestissementDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<FluxCreditCoopVM>> GetFlux()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.FluxCreditCoop.Select(f => new FluxCreditCoopVM
        {
            Id = f.Id,
            Date = f.Date,
            Valeur = f.Valeur,
            LibelleRecu = f.LibelleRecu,
            IdCategorie = f.Categorie == null ? 0 : f.Categorie.Id
        }).ToListAsync();
    }

    public async Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.CategorieFlux
            .Select(c => new CategorieFluxDto
            {
                Id = c.Id,
                Libelle = c.MicroCategorie,    
            })
            .OrderBy(f => f.Libelle)
            .ToListAsync();
    }

    public async Task<IEnumerable<BudgetLineChartVM>> CalculerBudgetCategorieParMois()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var rawData = await context.FluxCreditCoop
            .Where(f => f.IdCategorie != null)
            .Include(f => f.Categorie)
            .Where(f => f.Categorie.MacroCategorie != null)
            .GroupBy(t => new { t.Date.Year, t.Date.Month, t.Categorie.MacroCategorie })
            .Select(d => new
            {
                Categorie = d.Key.MacroCategorie,
                Date = new DateTime(d.Key.Year, d.Key.Month, 1),
                BudgetCategorie = d.Sum(f => f.Valeur)
            })
            .ToListAsync();

        return rawData
            .GroupBy(r => r.Categorie)
            .Select(r => new BudgetLineChartVM
            {
                Categorie = r.Key,
                BudgetCategorieParMois = r.Select(r => new BudgetCategorieParMoisVM
                {
                    Date = r.Date,
                    Budget = r.BudgetCategorie
                }).OrderBy(r => r.Date).ToList()
            })
            .ToList();
    }

    public async Task AddFluxCreditCoop(List<FluxCreditCoopApiResponse>? fluxCreditCoop)
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
            .Where(f => !idsExistants.Contains(f.Id))
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

    public async Task UpdateFluxCreditCoopMensuel(List<ViewsModels.FluxCreditCoopVM> fluxMensuelVM)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var idVM = fluxMensuelVM.Select(f => f.Id);

        var fluxMensuelEnregistree = await context.FluxCreditCoop
            .Where(f => idVM.Contains(f.Id))
            .ToListAsync();

        var fluxDic = fluxMensuelEnregistree.ToDictionary(e => e.Id);

        foreach (var fluxVm in fluxMensuelVM)
        {
            if (fluxDic.TryGetValue(fluxVm.Id, out var _fluxEnregistre))
            {
                _fluxEnregistre.LibelleRecu = fluxVm.LibelleRecu;
                _fluxEnregistre.IdCategorie = fluxVm.IdCategorie == 0 ? null : fluxVm.IdCategorie;
            }
        }

        await context.SaveChangesAsync();
    }
}