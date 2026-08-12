namespace Investissement_WebClient.Domain.Modeles
{
    public class Actif
    {
        public int Id { get; set; }

        public required string Libelle { get; set; }

        public required string ISIN { get; init; }

        public required string Ticker { get; set; }
    }
}
