using Investissement_WebClient.Application.InterfacesRepositories;
using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Repositories
{
    public class FluxBancaireRepository(IDbContextFactory<InvestissementDbContext> dbContext) : IFluxBancaireRepository
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;

        public async Task<DateTime?> GetDateDernierFluxByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.FluxBancaire
                .Where(f => f.UtilisateurId == userId)
                .OrderByDescending(f => f.Date)
                .Select(f => (DateTime?)f.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FluxBancaire>> GetByUserId(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.FluxBancaire
                .Where(f => f.UtilisateurId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FluxBancaire>> GetAllSansCategorie()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxBancaire
                .Where(f => f.IdCategorie == null)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int?>> GetCorrespondancesCategories()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxBancaire
                .Where(f => f.IdCategorie != null)
                .GroupBy(f => f.Libelle.Trim().ToLower())
                .ToDictionaryAsync(g => g.Key, g => g.First().IdCategorie);
        }

        public async Task<List<BudgetCategorieRawDto>> GetBudgetParMacroCategorieParMois(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxBancaire
                .Where(f => f.UtilisateurId == userId)
                .Where(f => f.IdCategorie != null)
                .Where(f => f.Categorie!.MacroCategorie != null)
                .GroupBy(t => new
                {
                    t.Date.Year,
                    t.Date.Month,
                    t.Categorie!.MacroCategorie
                })
                .Select(d => new BudgetCategorieRawDto
                {
                    Categorie = d.Key.MacroCategorie!,
                    Date = new DateTime(d.Key.Year, d.Key.Month, 1),
                    BudgetCategorie = d.Sum(f => f.Valeur)
                })
                .ToListAsync();
        }

        public async Task AddRangeForUserId(List<PowensFluxApiResponse> flux, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var idsExistants = (await context.FluxBancaire
                .Where(f => f.UtilisateurId == userId)
                .Select(f => f.Id)
                .ToListAsync())
                .ToHashSet();

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

        public async Task UpdateRangeForUserId(List<FluxBancaireVM> fluxMensuelVM, int userId)
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
                if (fluxDic.TryGetValue(fluxVm.Id, out var fluxEnregistre))
                {
                    fluxEnregistre.Libelle = fluxVm.Libelle;
                    fluxEnregistre.IdCategorie = fluxVm.IdCategorie == 0 ? null : fluxVm.IdCategorie;
                    fluxEnregistre.Suggestion = false;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
