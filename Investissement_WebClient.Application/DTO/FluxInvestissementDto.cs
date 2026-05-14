namespace Investissement_WebClient.Application.DTO
{
    public class FluxInvestissementDto
    {
        public DateTime Date { get; set; }

        public required string Actif { get; set; }
        
        public required string Ticker { get; set; }

        public decimal Prix { get; set; }

        public decimal Quantite { get; set; }
    }
}
