using Investissement_WebClient.Application.Services.Authentification;
using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class ConnexionViewModel(IAuthentificationService authentificationService)
    {
        private readonly IAuthentificationService _authentificationService = authentificationService;

        public ConnexionVM InformationsConnexion { get; set; } = new ConnexionVM();

        public bool HasErreur { get; set; } = false;
        public string MessageErreur { get; set; } = string.Empty;

        public async Task TentativeConnexion()
        {
            try
            {
                await _authentificationService.Connexion(InformationsConnexion);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }
    }
}
