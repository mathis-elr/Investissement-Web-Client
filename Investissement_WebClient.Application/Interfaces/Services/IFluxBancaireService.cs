using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.DTO;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IFluxBancaireService
    {
        Task<DateTime?> GetDateLimiteValiditeSyncBanque(int userId);

        Task<List<FluxBancaireVM>> GetFluxBancaire(int userId);

        Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

        Task VerifierEtSynchroniserFluxBancairesAsync();

        Task<IEnumerable<BudgetsParCategorieVM>> CalculerBudgetCategorieParMois(int userId);

        Task AddFluxBancaire(List<FluxBancaireImportDto>? flux, int userId);

        Task UpdateFluxMensuel(List<FluxBancaireVM> fluxMensuelVM, int userId);
    }
}

