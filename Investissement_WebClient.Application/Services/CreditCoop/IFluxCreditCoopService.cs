using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Application.Services.CreditCoop;

public interface IFluxCreditCoopService
{

    Task<List<FluxCreditCoopVM>> GetFlux();

    Task<IEnumerable<CategorieFluxVM>> GetCategorieFlux();

    Task AddFluxCreditCoop(List<FluxCreditCoopDto>? fluxCreditCoop);

    Task UpdateFluxCreditCoopMensuel(List<FluxCreditCoopVM> fluxMensuelVM);

}