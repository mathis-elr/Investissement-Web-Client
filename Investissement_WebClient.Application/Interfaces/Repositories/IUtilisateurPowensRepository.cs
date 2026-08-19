using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IUtilisateurPowensRepository
    {
        Task<UtilisateurPowens?> GetByUserId(int userId);

        Task Add(UtilisateurPowens acces);

        Task Update(UtilisateurPowens acces);
    }
}
