namespace Investissement_WebClient.Application.DTO.FluxBancaires
{
    public class FluxBancaireImportDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Valeur { get; set; }

        public string Libelle { get; set; } = string.Empty;
    }
}
