using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services
{
    public class FluxBancaireService(ICategorieFluxRepository categorieFluxRepository,
                                     IFluxBancaireRepository fluxBancaireRepository,
                                     IBanqueAccesRepository banqueAccesRepository,
                                     IPowensApiService powensApiService) : IFluxBancaireService
    {
        private readonly ICategorieFluxRepository _categorieFluxRepository = categorieFluxRepository;
        private readonly IBanqueAccesRepository _banqueAccesRepository = banqueAccesRepository;
        private readonly IFluxBancaireRepository _fluxBancaireRepository = fluxBancaireRepository;
        private readonly IPowensApiService _powensApiService = powensApiService;

        public async Task<DateTime?> GetDateLimiteValiditeSyncBanque(int userId)
        {
            var acces = await _banqueAccesRepository.GetByUserId(userId);
            return acces?.DateExpiration;
        }

        public async Task<DateTime?> GetDateDernierFlux(int userId)
        {
            return await _fluxBancaireRepository.GetDateDernierFluxByUserId(userId);
        }

        public async Task<List<FluxBancaireVM>> GetFluxBancaire(int userId)
        {
            var flux = await _fluxBancaireRepository.GetByUserId(userId);

            return flux.Select(f => new FluxBancaireVM
            {
                Id = f.Id,
                Date = f.Date,
                Valeur = f.Valeur,
                Libelle = f.Libelle,
                IdCategorie = f.Categorie == null ? 0 : f.Categorie.Id,
                Suggestion = f.Suggestion
            }).ToList();
        }

        public async Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux()
        {
            var categories = await _categorieFluxRepository.GetAll();

            return categories.Select(c => new CategorieFluxDto
            {
                Id = c.Id,
                Libelle = c.MicroCategorie,
            })
                .OrderBy(f => f.Libelle)
                .ToList();
        }

        public async Task VerifierEtSynchroniserFluxBancairesAsync()
        {
            var currentDate = DateTime.Now;

            var finMoisPrecedent = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-1);

            var idsUsers = await _banqueAccesRepository.GetAll();

            foreach (var idUser in idsUsers)
            {
                var derniereDate = await GetDateDernierFlux(idUser);

                if (derniereDate.HasValue && derniereDate.Value >= finMoisPrecedent)
                    continue;

                var dateDebut = derniereDate ?? new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-2);

                await _powensApiService.GetFlux(dateDebut, finMoisPrecedent, idUser);
            }
        }

        public async Task DeterminerCategorieFlux()
        {
            var fluxSansCategorie = await _fluxBancaireRepository.GetAllSansCategorie();
            var dicCorrespondanceFluxBancaire = await _fluxBancaireRepository.GetCorrespondancesCategories();

            var fluxModifies = new List<FluxBancaire>();

            foreach (var flux in fluxSansCategorie)
            {
                if (dicCorrespondanceFluxBancaire.TryGetValue(flux.Libelle.ToLower(), out int? idCategorie))
                {
                    flux.IdCategorie = idCategorie;
                    flux.Suggestion = true;
                    fluxModifies.Add(flux);
                }
            }

            if (fluxModifies.Count != 0)
                await _fluxBancaireRepository.UpdateRangeSuggestions(fluxModifies);
        }

        public async Task<IEnumerable<BudgetsParCategorieVM>> CalculerBudgetCategorieParMois(int userId)
        {
            var rawData = await _fluxBancaireRepository.GetBudgetParMacroCategorieParMois(userId);

            var moisPossibles = rawData
                .Select(r => r.Date)
                .Distinct()
                .OrderBy(d => d);

            return rawData
                .GroupBy(r => r.Categorie!)
                .Select(g =>
                {
                    var budgetsParDate = g.ToDictionary(
                        x => x.Date,
                        x => x.BudgetCategorie);

                    return new BudgetsParCategorieVM
                    {
                        Categorie = g.Key,
                        BudgetCategorieParMois = moisPossibles
                            .Select(m => new BudgetParMoisLineChartVM
                            {
                                Date = m,
                                Budget = budgetsParDate.GetValueOrDefault(m, 0)
                            })
                            .ToList()
                    };
                })
                .OrderByDescending(f => f.BudgetCategorieParMois.Sum(b => b.Budget))
                .ToList();
        }

        public async Task AddFluxBancaire(List<FluxBancaireImportDto>? flux, int userId)
        {
            if (flux == null || flux.Count == 0)
                return;

            await _fluxBancaireRepository.AddRangeForUserId(flux, userId);

            await DeterminerCategorieFlux();
        }

        public async Task UpdateFluxMensuel(List<FluxBancaireVM> fluxMensuelVM, int userId)
        {
            await _fluxBancaireRepository.UpdateRangeForUserId(fluxMensuelVM, userId);
        }
    }
}