using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Domain.Modeles
{
    public class Banque
    {
        public int Id { get; set; }

        public required int IdConnectionPowens { get; set; }

        public required int IdConnectorPowens { get; set; }

        public required string Nom { get; set; }

        public StatutConnexion StatutConnexion { get; set; }
        public DateTime? DerniereSynchro { get; set; }

        public ICollection<CompteBanque> Comptes { get; set; } = [];

        public int UtilisateurPowensId { get; set; }
        public UtilisateurPowens UtilisateurPowens { get; set; } = null!;
    }
}
