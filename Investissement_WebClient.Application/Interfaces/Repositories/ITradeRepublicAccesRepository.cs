using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ITradeRepublicAccesRepository
    {
        Task<SourceDto?> GetByUserId(int userId);

        Task<CompteTradeRepublic?> GetLoginByUserId(int userId);

        Task Add(CompteTradeRepublic acces);
    }
}
