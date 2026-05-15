using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Application.ViewsModels
{
    public class ConnexionVM
    {
        [Required(ErrorMessage = "L'adresse email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit faire au moins 6 caractères.")]
        public string Mdp { get; set; } = string.Empty;
    }
}
