namespace Investissement_WebClient.Application.DTO
{
    public class PositionInvestissementImportDto
    {
        public string Code {  get; set; } = string.Empty;
        public bool ISIN { get; set; }
        public string Label { get; set; } = string.Empty;

        public decimal Quantite { get; set; }
        public decimal PrixAchat { get; set; }
        public decimal PrixCourant { get; set; }
        public DateTime DatePrixCourant { get; set; }

        public DateTime DerniereMaj { get; set; }

        public int CompteBanqueId { get; set; }
    }
}
