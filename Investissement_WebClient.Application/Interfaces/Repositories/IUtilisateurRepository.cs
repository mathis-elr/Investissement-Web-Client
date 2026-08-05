using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Repositories
{
    public interface IUtilisateurRepository
    {
        Task<Utilisateur?> GetByEmail(string email);

        Task<int> Add(Utilisateur utilisateur);
    }
}
