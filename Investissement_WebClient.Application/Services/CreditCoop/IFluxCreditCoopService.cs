using Investissement_WebClient.Application.ApiResponse;
using Investissement_WebClient.Application.DTO;

namespace Investissement_WebClient.Application.Services.CreditCoop;

public interface IFluxCreditCoopService
{

    Task<List<ViewsModels.FluxCreditCoopVM>> GetFlux();

    Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

    Task AddFluxCreditCoop(List<FluxCreditCoopApiResponse>? fluxCreditCoop);

    Task UpdateFluxCreditCoopMensuel(List<ViewsModels.FluxCreditCoopVM> fluxMensuelVM);

}