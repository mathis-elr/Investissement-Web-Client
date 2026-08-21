using Investissement_WebClient.Application.DTO.FluxBancaires;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface ICompteBanqueService
    {
        Task<List<SourceDto>> GetAllByUserId(int userId);
    }
}
