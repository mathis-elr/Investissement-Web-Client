using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.DTO.FluxBancaires
{
    public class CompteBanqueDto
    {
        public int Id { get; set; }

        public int IdComptePowens { get; set; }

        public string Nom { get; set; } = string.Empty;

        public string TypePowens { get; set; } = string.Empty;

        public TypeCompte TypeCompte { get; set; }

        public Banque Banque { get; set; } = null!;

        public bool Selectionne { get; set; } = false;
    }
}
