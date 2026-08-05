using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.DTO
{
    public class FluxInvestissementImportDto
    {
        public string Id { get; set; } = string.Empty;

        public DateTimeOffset Date { get; set; }

        public TypeFlux Type { get; set; }

        public decimal Prix { get; set; }

        public decimal Quantite { get; set; }

        public decimal? Frais { get; set; }

        public decimal? Total { get; set; }

        public string ISIN { get; set; } = string.Empty;

        public string Actif { get; set; } = string.Empty;
    }
}
