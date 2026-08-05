namespace Investissement_WebClient.Infrastructure.APIs.TradeRepublic.Responses
{
    public class TradeRepublicUnFluxApiResponse
    {
        public string? Id { get; init; }

        public DateTimeOffset? Date { get; init; }

        public int? Type { get; init; }

        public string? Actif { get; set; }

        public string? ISIN { get; init; }

        public string? Ticker { get; set; }

        public decimal? Prix { get; init; }

        public decimal? Quantite { get; init; }

        public decimal? Frais { get; init; }

        public decimal? Total { get; init; }
    }
}
