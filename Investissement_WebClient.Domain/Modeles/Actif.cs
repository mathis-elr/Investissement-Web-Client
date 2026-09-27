namespace Investissement_WebClient.Domain.Modeles
{
    public class Actif
    {
        public int Id { get; set; }

        public required string Libelle { get; set; }

        public string? ISIN { get; init; }

        public string? Ticker { get; set; }
    }
}
