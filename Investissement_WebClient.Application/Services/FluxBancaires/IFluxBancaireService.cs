using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;

namespace Investissement_WebClient.Application.Services.FluxBancaires;

public interface IFluxBancaireService
{
    Task<DateTime?> GetDateLimiteValiditeSyncBanque(int userId);

    Task<List<FluxBancaireVM>> GetFluxBancaire(int userId);

    Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

    Task<IEnumerable<BudgetsParCategorieVM>> CalculerBudgetCategorieParMois(int userId);

    Task AddFluxBancaire(List<PowensFluxApiResponse>? flux, int userId);

    Task UpdateFluxMensuel(List<FluxBancaireVM> fluxMensuelVM, int userId);

}