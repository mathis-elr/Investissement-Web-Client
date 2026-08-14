namespace Investissement_WebClient.Domain.Modeles
{
    public class BanqueAcces
    {
        public int Id { get; set; }

        public required string AccesTokenCrypte { get; set; }

        public int IdCompteCourant { get; set; }

        public DateTime DateCreation { get; set; }

        public DateTime DateExpiration { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
