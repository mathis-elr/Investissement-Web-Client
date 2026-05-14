namespace Investissement_WebClient.Application.DTO
{
    public class InfoValeurParActifDto
    {
        public required string Actif { get; set; }

        public decimal ValeurDetenue { get; set; }

        public decimal VariationPourcentage { get; set; }

        public decimal VariationValeur { get; set; }
    }
}
