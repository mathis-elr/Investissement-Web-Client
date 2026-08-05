using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IAuthentificationService
    {
        Task<int> Inscription(InscriptionVM infosInscription);

        Task<Utilisateur> Connexion(ConnexionVM infosConnexion);
    }
}
