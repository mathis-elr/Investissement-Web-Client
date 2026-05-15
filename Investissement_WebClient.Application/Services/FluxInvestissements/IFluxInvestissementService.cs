using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.Services.FluxInvestissements
{
    public interface IFluxInvestissementService
    {
        Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement(int userId);

        Task<string?> GetDernierFluxEnregistre(int userId);

        Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(decimal investissementMoyenMensuel, int userId);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif, int userId);

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif, int userId);
    
        Task<decimal> CalculerValeurInvestissementTotal(int userId);

        Task<decimal> CalculerInvestissementMoyenMensuel(int userId);

        Task<IEnumerable<InfoValeurParActifDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif, int userId);

        Task MapperTransactions(List<TradeRepublicUnFluxApiResponse> transactions, int userId);
    }
}
