namespace Investissement_WebClient.Application.DTO.Patrimoine
{
    public class PositionPatrimoineUtilisateurDto
    {
        public int UtilisateurId { get; set; }

        public string Ticker { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public decimal Quantite { get; set; }
    }
}