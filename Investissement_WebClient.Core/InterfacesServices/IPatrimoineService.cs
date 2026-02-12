using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IPatrimoineService
{
    Task<double> CalculerValeurPatrimoineCourante();

    Task<VariationsDto> GetVariations(double valeurActuelle);
}