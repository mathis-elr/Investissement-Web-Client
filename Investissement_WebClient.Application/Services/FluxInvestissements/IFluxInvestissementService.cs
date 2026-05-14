using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.Services.FluxInvestissements
{
    public interface IFluxInvestissementService
    {
        Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement();

        Task<string?> GetDernierFluxEnregistre();

        Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(decimal investissementMoyenMensuel);

        Task<Dictionary<string, decimal>> GetPrixParActif();

        Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif);

        Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif);
    
        Task<decimal> CalculerValeurInvestissementTotal();

        Task<decimal> CalculerInvestissementMoyenMensuel();

        Task<IEnumerable<InfoValeurParActifDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif);

        Task MapperTransactions(List<TradeRepublicUnFluxApiResponse> transactions);
    }
}
