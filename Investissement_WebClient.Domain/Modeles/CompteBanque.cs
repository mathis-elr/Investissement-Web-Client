using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Domain.Modeles
{
    public class CompteBanque
    {
        public int Id { get; set; }

        public int IdComptePowens { get; set; }

        public string TypePowens { get; set; } = string.Empty;

        public TypeCompte TypeCompte { get; set; }

        public string Nom { get; set; } = string.Empty;

        public decimal Solde { get; set; }

        public int BanqueId { get; set; }

        public Banque Banque { get; set; } = null!;
    }
}
