using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ITradeRepublicAccesRepository
    {
        Task<TradeRepublicAcces?> GetByUserId(int userId);

        Task Add(TradeRepublicAcces acces);
    }
}
