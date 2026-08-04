using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.InterfacesRepositories
{
    public interface ITradeRepublicAccesRepository
    {
        Task<TradeRepublicAcces?> GetByUserId(int userId);

        Task Add(TradeRepublicAcces acces);
    }
}
