namespace Investissement_WebClient.Application.ApiResponse.TradeRepublic
{
    public class TradeRepublicUnFluxApiResponse
    {
        public string? Id { get; init; }

        public DateTimeOffset? Date { get; init; }

        public string? Type { get; init; }

        public string? Actif { get; set; }

        public string? ISIN { get; init; }

        public string? Ticker { get; set; }

        public decimal? Prix { get; init; }

        public decimal? Quantite { get; init; }

        public decimal? Frais { get; init; }

        public decimal? Total { get; init; }
    }
}
