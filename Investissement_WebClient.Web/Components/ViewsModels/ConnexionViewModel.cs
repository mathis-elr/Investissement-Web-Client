using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Auth;
using Investissement_WebClient.Web.GestionSession;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class ConnexionViewModel(IAuthentificationService authentificationService)
    {
        private readonly IAuthentificationService _authentificationService = authentificationService;

        public ConnexionDto InformationsConnexion { get; set; } = new ConnexionDto();
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
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }
    }
}
