using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;
using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Application.Services.API.PowensApi;

namespace Investissement_WebClient.Application.Services.FluxBancaires;

public class FluxBancaireService(IDbContextFactory<InvestissementDbContext> dbFactory, IPowensApiService powensApiService) : IFluxBancaireService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbFactory;
    private readonly IPowensApiService _powensApiService = powensApiService;

    public async Task<DateTime?> GetDateLimiteValiditeSyncBanque(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var acces = await context.BanqueAcces
            .Where(b => b.UtilisateurId == userId)
            .FirstOrDefaultAsync();
        return acces?.DateExpiration;
    }

    public async Task<DateTime?> GetDateDernierFlux(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.FluxBancaire
            .Where(f => f.UtilisateurId == userId)
            .OrderByDescending(f => f.Date)
            .Select(f => (DateTime?)f.Date)
            .FirstOrDefaultAsync();
    }

    public async Task<List<FluxBancaireVM>> GetFluxBancaire(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.FluxBancaire
            .Where(f => f.UtilisateurId == userId)
            .Select(f => new FluxBancaireVM
            {
                Id = f.Id,
            Date = f.Date,
            Valeur = f.Valeur,
            Libelle = f.Libelle,
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

    public async Task VerifierEtSynchroniserFluxBancairesAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var currentDate = DateTime.Now;

        var finMoisPrecedent = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-1);

        var idsUsers = await context.BanqueAcces
            .Where(b => b.DateExpiration > DateTime.Now)
            .Select(b => b.UtilisateurId)
            .ToListAsync();

        foreach (var idUser in idsUsers)
        {
            var derniereDate = await GetDateDernierFlux(idUser);

            if (derniereDate.HasValue && derniereDate.Value >= finMoisPrecedent)
                continue;

            var dateDebut = derniereDate ?? new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-2);

            await _powensApiService.GetFlux(dateDebut, finMoisPrecedent, idUser);
        }
    }

    public async Task<IEnumerable<BudgetsParCategorieVM>> CalculerBudgetCategorieParMois(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var rawData = await context.FluxBancaire
            .Where(f => f.UtilisateurId == userId)
            .Where(f => f.IdCategorie != null)
            .Include(f => f.Categorie)
            .Where(f => f.Categorie!.MacroCategorie != null)
            .GroupBy(t => new { t.Date.Year, t.Date.Month, t.Categorie!.MacroCategorie })
            .Select(d => new
            {
                Categorie = d.Key.MacroCategorie,
                Date = new DateTime(d.Key.Year, d.Key.Month, 1),
                BudgetCategorie = d.Sum(f => f.Valeur)
            })
            .ToListAsync();

        var moisPossibles = rawData.GroupBy(f => f.Date).Select(r => r.Key).OrderBy(d => d.Date);

        return rawData
            .GroupBy(r => r.Categorie!)
            .Select(r => new BudgetsParCategorieVM
            {
                Categorie = r.Key,
                BudgetCategorieParMois = moisPossibles.Select(m => new BudgetParMoisLineChartVM
                {
                    Date = m.Date,
                    Budget = r.FirstOrDefault(r => r.Date == m.Date)?.BudgetCategorie ?? 0
                }).OrderBy(m => m.Date).ToList()
            }).OrderByDescending(f => f.BudgetCategorieParMois.Sum(b => b.Budget))
            .ToList();
    }

    public async Task AddFluxBancaire(List<PowensFluxApiResponse>? flux, int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        if (flux == null || flux.Count == 0)
            return;

        var idsExistants = await context.FluxBancaire
            .Where(f => f.UtilisateurId == userId)
            .Select(f => f.Id)
            .ToListAsync();

        var nvFlux = flux
            .Where(f => !idsExistants.Contains(f.Id))
            .Select(f => new FluxBancaire
            {
                Id = f.Id,
                Date = f.Date,
                Valeur = f.Valeur,
                Libelle = f.Libelle ?? string.Empty,
                UtilisateurId = userId
            });

        context.FluxBancaire.AddRange(nvFlux);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFluxMensuel(List<FluxBancaireVM> fluxMensuelVM, int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var idVM = fluxMensuelVM.Select(f => f.Id);

        var fluxMensuelEnregistree = await context.FluxBancaire
            .Where(f => idVM.Contains(f.Id))
            .Where(f => f.UtilisateurId == userId)
            .ToListAsync();

        var fluxDic = fluxMensuelEnregistree.ToDictionary(e => e.Id);

        foreach (var fluxVm in fluxMensuelVM)
        {
            if (fluxDic.TryGetValue(fluxVm.Id, out var _fluxEnregistre))
            {
                _fluxEnregistre.Libelle = fluxVm.Libelle;
                _fluxEnregistre.IdCategorie = fluxVm.IdCategorie == 0 ? null : fluxVm.IdCategorie;
            }
        }

        await context.SaveChangesAsync();
    }
}