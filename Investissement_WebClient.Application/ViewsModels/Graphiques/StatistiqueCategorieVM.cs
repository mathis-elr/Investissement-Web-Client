namespace Investissement_WebClient.Application.ViewsModels.Graphiques
{
    public class StatistiqueCategorieVM
    {
        public string NomCategorie { get; set; } = string.Empty;

        public decimal TotalCredit { get; set; }

        public decimal TotalDebit { get; set; }
    }
}
