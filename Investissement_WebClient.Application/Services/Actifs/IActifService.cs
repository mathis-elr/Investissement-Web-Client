using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services.Actifs
{
    public interface IActifService
    {
        Task<List<Actif>> GetAll();

        Task<IEnumerable<string>> GetTickers();

        Task<int> AddActif(Actif actif);
    }
}
