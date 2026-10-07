namespace Investissement_WebClient.Application.DTO.FluxTradeRepublic
{
    public class FluxTradeRepublicDto
    {
        public DateTime Date { get; set; }

        public required string Actif { get; set; }
        
        public required string Ticker { get; set; }

        public string? Logo { get; set; }

        public decimal Prix { get; set; }

        public decimal Quantite { get; set; }
    }
}
