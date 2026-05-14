using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;

namespace Investissement_WebClient.Application.Services.FluxBancaires;

public interface IFluxBancaireService
{
    Task<DateTime?> GetDateLimiteValiditeSyncBanque();

    Task<List<FluxBancaireVM>> GetFluxBancaire();

    Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

    Task<IEnumerable<BudgetsParCategorieVM>> CalculerBudgetCategorieParMois();

    Task AddFluxBancaire(List<PowensFluxApiResponse>? flux);

    Task UpdateFluxCreditCoopMensuel(List<FluxBancaireVM> fluxMensuelVM);

}