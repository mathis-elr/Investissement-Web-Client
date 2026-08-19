namespace Investissement_WebClient.Domain.Modeles
{
    public class UtilisateurPowens
    {
        public int Id { get; set; }

        public int IdUtilisateurPowens { get; set; }

        public required string AccessTokenCrypte { get; set; }

        public ICollection<Banque> Banques { get; set; } = [];

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
