using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Application.Services.Authentification
{
    public interface IAuthentificationService
    {
        Task Inscription(InscriptionVM infosInscription);

        Task Connexion(ConnexionVM infosConnexion);
    }
}
