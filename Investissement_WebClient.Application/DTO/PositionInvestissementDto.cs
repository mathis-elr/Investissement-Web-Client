namespace Investissement_WebClient.Application.DTO
{
    public class PositionInvestissementDto
    {
        public string Actif { get; set; } = string.Empty;

        public string Ticker { get; set; } = string.Empty;

        public decimal TotalQuantite { get; set; }

        public decimal TotalValeurInvestie { get; set; }
    }
}
