namespace Investissement_WebClient.Application.DTO
{
    public class InfoInvestParActifDto
    {
        public string? Actif { get; set; }

        public decimal? QuantiteDetenue { get; set; }

        public decimal? PrixAchatMoyen { get; set; }

        public decimal? PrixActuel { get; set; }

        public decimal? ValeurDetenue { get; set; }

        public decimal? VariationPourcentage { get; set; }

        public decimal? VariationValeur { get; set; }
    }
}
