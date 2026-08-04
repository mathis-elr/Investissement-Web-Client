namespace Investissement_WebClient.Application.DTO
{
    public class PositionActifDto
    {
        public string Actif { get; set; } = string.Empty;

        public string Ticker { get; set; } = string.Empty;

        public decimal QuantiteTotale { get; set; }
    }
}
