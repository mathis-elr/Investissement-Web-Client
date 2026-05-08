using ApexCharts;
using Investissement_WebClient.Application.ViewsModels;

namespace Investissement_WebClient.Web.Components.Views.Budget;

public static class ChartsOptionsRecapBudgetMensuel
{
    public static ApexChartOptions<FluxCreditCoopVM> OptionsRecapitulatifBudgetMensuel = new()
    {
        
        Stroke = new Stroke
        {
            Curve = Curve.Smooth, Width = 3
        },

        Chart = new Chart
        {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Toolbar = new Toolbar { Show = false } ,
            Height = "500px"
        },

        Grid = new Grid
        {
            BorderColor = "#444" 
        },
        
        Tooltip = new Tooltip
        {
            Theme = Mode.Dark,
        },
    };  
}