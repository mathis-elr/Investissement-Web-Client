namespace Investissement_WebClient.Application.DTO
{
    public class FluxInvestissementDto
    {
        public DateTime Date { get; set; }

        public string? Actif { get; set; }
        
        public string? Ticker { get; set; }

        public decimal? Prix { get; set; }

        public decimal? Quantite { get; set; }
    }
}
