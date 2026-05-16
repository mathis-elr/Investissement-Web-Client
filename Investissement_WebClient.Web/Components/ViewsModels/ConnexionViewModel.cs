using Investissement_WebClient.Application.Services.Authentification;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Web.GestionSession;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class ConnexionViewModel(IAuthentificationService authentificationService)
    {
        private readonly IAuthentificationService _authentificationService = authentificationService;

        public ConnexionVM InformationsConnexion { get; set; } = new ConnexionVM();
        public SessionUtilisateur SessionUtilisateur { get; set; } = new SessionUtilisateur();

        public bool HasErreur { get; set; } = false;
        public string MessageErreur { get; set; } = string.Empty;

        public async Task TentativeConnexion()
        {
            try
            {
                var user = await _authentificationService.Connexion(InformationsConnexion);
                SessionUtilisateur.Id = user.Id;
                SessionUtilisateur.Email = user.Email;
                SessionUtilisateur.Prenom = user.Prenom;
                SessionUtilisateur.DateCreationCompte = user.DateCreationCompte;
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }
    }
}
