namespace Investissement_WebClient.Domain.Modeles
{
    public class CompteTradeRepublic
    {
        public int Id { get; set; }

        public required string NumTelCrypte { get; set; }

        public required string PinCrypte { get; set; }

        public DateTime? DerniereSynchronisation { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
