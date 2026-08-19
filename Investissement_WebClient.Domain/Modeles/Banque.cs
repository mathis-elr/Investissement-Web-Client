namespace Investissement_WebClient.Domain.Modeles
{
    public class Banque
    {
        public int Id { get; set; }

        public required int IdConnectionPowens { get; set; }

        public required int IdConnectorPowens { get; set; }

        public required string Nom { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime DateExpiration { get; set; }

        public ICollection<CompteBanque> Comptes { get; set; } = [];

        public int UtilisateurPowensId { get; set; }
        public UtilisateurPowens UtilisateurPowens { get; set; } = null!;
    }
}
