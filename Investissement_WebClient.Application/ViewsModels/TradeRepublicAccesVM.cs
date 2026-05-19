using System.ComponentModel.DataAnnotations;

namespace Investissement_WebClient.Application.ViewsModels
{
    public class TradeRepublicAccesVM
    {
        private string _numTel = string.Empty;


        [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string NumTel
        {
            get => _numTel;
            set
            {
                var seulementChiffres = new string(value.Where(char.IsDigit).ToArray());

                if (seulementChiffres.Length > 9)
                {
                    seulementChiffres = seulementChiffres.Substring(0, 9);
                }

                _numTel = FormaterTelephone(seulementChiffres);
            }
        }

        [Required(ErrorMessage = "Le code PIN est requis.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Le code PIN doit comporter exactement 4 chiffres.")]
        public string Pin { get; set; } = string.Empty;


        private string FormaterTelephone(string chiffres)
        {
            if (string.IsNullOrEmpty(chiffres)) return string.Empty;

            var resultat = new System.Text.StringBuilder();
            for (int i = 0; i < chiffres.Length; i++)
            {
                if (i == 1 || i == 3 || i == 5 || i == 7)
                {
                    resultat.Append(' ');
                }
                resultat.Append(chiffres[i]);
            }
            return resultat.ToString();
        }
    }
}
