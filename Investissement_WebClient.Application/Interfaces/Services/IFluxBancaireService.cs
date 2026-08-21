using Investissement_WebClient.Application.DTO.FluxBancaires;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IFluxBancaireService
    {
        Task<DateTime?> GetDateDernierFlux(int compteId);

        Task<List<FluxBancaireDto>> GetFluxBancaire(int userId);

        Task<List<FluxBancaireDto>> GetFluxByCompteId(int compteId);

        Task<IEnumerable<CategorieFluxDto>> GetCategorieFlux();

        Task<IEnumerable<BudgetsParCategorieDto>> CalculerBudgetCategorieParMois(int compteId);

        Task AddFluxBancaire(List<FluxBancaireImportDto>? flux, int userId, int comptePowensId);

        Task UpdateFluxMensuel(List<FluxBancaireDto> fluxMensuelVM, int userId);
    }
}

