namespace Investissement_WebClient.Domain.Modeles
{
    public class TradeRepublicAcces
    {
        public int Id { get; set; }

        public required string NumTel { get; set; }

        public required int PinCrypte { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
