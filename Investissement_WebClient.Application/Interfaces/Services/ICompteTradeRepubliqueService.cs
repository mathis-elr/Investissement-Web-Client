using Investissement_WebClient.Application.DTO.FluxBancaires;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface ICompteTradeRepubliqueService
    {
        Task<SourceDto?> GetByUserId(int userId);
    }
}
