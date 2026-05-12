namespace Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets
{
    public class BudgetsParCategorieVM
    {
        public required string Categorie { get; set; }

        public List<BudgetParMoisLineChartVM> BudgetCategorieParMois { get; set; } = [];
    }
}
