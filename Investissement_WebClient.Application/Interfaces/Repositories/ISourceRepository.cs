using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface ISourceRepository
    {
        Task<List<Source>> GetAllByUserId(int userId);

        Task<int> Add(Source source);

        Task Update(Source source);
    }
}
