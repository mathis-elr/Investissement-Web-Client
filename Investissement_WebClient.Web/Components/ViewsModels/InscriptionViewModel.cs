using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Auth;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class InscriptionViewModel(IAuthentificationService authentificationService)
    {
        private readonly IAuthentificationService _authentificationService = authentificationService;

        public InscriptionDto InformationsInscription { get; set; } = new InscriptionDto();

        public bool HasErreur { get; set; } = false;
        public string MessageErreur { get; set; } = string.Empty;

        public async Task TentativeInscription()
        {
            if(InformationsInscription.Mdp != InformationsInscription.ConfirmationMdp)
            {
                HasErreur = true;
                MessageErreur = "Les mots de passe ne correspondent pas.";
                return;
            }

            try
            {
                InformationsInscription.Id = await _authentificationService.Inscription(InformationsInscription);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }
    }
}
