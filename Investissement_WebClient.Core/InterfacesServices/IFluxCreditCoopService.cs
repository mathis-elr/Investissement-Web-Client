using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IFluxCreditCoopService
{
    Task AddFluxCreditCoop(List<FluxCreditCoopDto>? fluxCreditCoop);
}