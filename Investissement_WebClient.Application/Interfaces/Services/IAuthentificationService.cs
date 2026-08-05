using Investissement_WebClient.Application.DTO.Auth;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IAuthentificationService
    {
        Task<int> Inscription(InscriptionDto infosInscription);

        Task<Utilisateur> Connexion(ConnexionDto infosConnexion);
    }
}
