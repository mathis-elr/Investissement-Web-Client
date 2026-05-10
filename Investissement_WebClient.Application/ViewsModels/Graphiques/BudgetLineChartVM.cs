namespace Investissement_WebClient.Application.ViewsModels.Graphiques
{
    public class BudgetLineChartVM
    {
        public required string Categorie { get; set; }

        public List<BudgetCategorieParMoisVM> BudgetCategorieParMois { get; set; } = [];
    }
}
