namespace Investissement_WebClient.Domain.Modeles
{
    public class Utilisateur
    {
        public int Id { get; set; }

        public required string Prenom { get; set; }

        public required string Email { get; set; }

        public required string MdpHash { get; set; }

        public DateTime DateCreationCompte { get; set; }
    }
}
