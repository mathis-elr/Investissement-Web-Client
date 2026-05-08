namespace Investissement_WebClient.Application.ViewsModels
{
    public class TransactionVM
    {
        public DateTimeOffset Date { get; set; }

        public string? Actif { get; set; }
        
        public string? Ticker { get; set; }

        public decimal? Prix { get; set; }

        public decimal? Quantite { get; set; }
    }
}
