using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Application.ViewsModels
{
    public class TradeRepublicAccesVM
    {
        [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string NumTel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le code PIN est requis.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Le code PIN doit comporter exactement 4 chiffres.")]
        public string Pin { get; set; } = string.Empty;
    }
}
