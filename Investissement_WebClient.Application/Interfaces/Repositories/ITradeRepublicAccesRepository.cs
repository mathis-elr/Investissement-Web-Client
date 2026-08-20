using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ITradeRepublicAccesRepository
    {
        Task<CompteTradeRepublic?> GetByUserId(int userId);

        Task Add(CompteTradeRepublic acces);
    }
}
