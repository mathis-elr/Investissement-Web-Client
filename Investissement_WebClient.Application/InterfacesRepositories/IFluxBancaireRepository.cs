using Investissement_WebClient.Application.ApiResponse.Powens;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.InterfacesRepositories
{
    public interface IFluxBancaireRepository
    {
        Task<DateTime?> GetDateDernierFluxByUserId(int userId);

        Task<List<FluxBancaire>> GetByUserId(int userId);

        Task<IEnumerable<FluxBancaire>> GetAllSansCategorie();

        Task<Dictionary<string, int?>> GetCorrespondancesCategories();

        Task<List<BudgetCategorieRawDto>> GetBudgetParMacroCategorieParMois(int userId);

        Task AddRangeForUserId(List<PowensFluxApiResponse> flux, int userId);

        Task UpdateRangeForUserId(List<FluxBancaireVM> fluxMensuelVM, int userId);

        Task UpdateRangeSuggestions(IEnumerable<FluxBancaire> fluxList);
    }
}
