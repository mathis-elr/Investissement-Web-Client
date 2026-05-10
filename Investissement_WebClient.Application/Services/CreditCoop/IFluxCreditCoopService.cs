using Investissement_WebClient.Application.ApiResponse;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Services.CreditCoop;

public interface IFluxCreditCoopService
{

    Task<List<ViewsModels.FluxCreditCoopVM>> GetFlux();

    Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

    Task<IEnumerable<BudgetLineChartVM>> CalculerBudgetCategorieParMois();

    Task AddFluxCreditCoop(List<FluxCreditCoopApiResponse>? fluxCreditCoop);

    Task UpdateFluxCreditCoopMensuel(List<ViewsModels.FluxCreditCoopVM> fluxMensuelVM);

}