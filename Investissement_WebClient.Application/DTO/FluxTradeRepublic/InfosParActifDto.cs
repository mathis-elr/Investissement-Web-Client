namespace Investissement_WebClient.Application.DTO.FluxTradeRepublic
{

    public class InfoParActifDto
    {
        public required string Actif { get; set; }

        public string? Logo { get; set; }

        public decimal ValeurDetenue { get; set; }

        public decimal VariationPourcentage { get; set; }

        public decimal VariationValeur { get; set; }
    }
}
