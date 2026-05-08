using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.ViewsModels;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IFluxCreditCoopService
{

    Task<List<FluxCreditCoopVM>> GetFlux();

    Task<IEnumerable<CategorieFluxVM>> GetCategorieFlux();

    Task AddFluxCreditCoop(List<FluxCreditCoopDto>? fluxCreditCoop);

    Task UpdateFluxCreditCoopMensuel(List<FluxCreditCoopVM> fluxMensuelVM);

}