using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IFluxInvestissementService
    {
        Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement(int userId);

        Task<string?> GetDernierFluxEnregistre(int userId);

        Task<DateTime?> GetDatePremierFlux(int userId);

        Task<IEnumerable<InvestissementParMoisDto>> GetInvestissementParMois(PeriodeHistoriqueInvest periode, int userId);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<IEnumerable<ValeurTotaleParActifDto>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif, int userId);

        Task<IEnumerable<ValeurActifInfosDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif, int userId);

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif, int userId);
    
        Task<decimal> CalculerValeurInvestissementTotal(int userId);

        Task<decimal> CalculerInvestissementMedianMensuel(int userId);

        Task MapperTransactions(List<FluxInvestissementImportDto> transactions, int userId);
    }
}
