using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.DTO;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IFluxInvestissementService
    {
        Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement(int userId);

        Task<string?> GetDernierFluxEnregistre(int userId);

        Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(int userId);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<IEnumerable<ValeurActifInfosDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif, int userId);

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif, int userId);
    
        Task<decimal> CalculerValeurInvestissementTotal(int userId);

        Task<decimal> CalculerInvestissementMedianMensuel(int userId);

        Task MapperTransactions(List<FluxInvestissementImportDto> transactions, int userId);
    }
}
