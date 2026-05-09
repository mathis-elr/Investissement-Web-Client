using ApexCharts;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Charts;

public static class ChartsOptionsRecapBudgetMensuel
{
    public static ApexChartOptions<StatistiqueCategorieVM> OptionsRecapitulatifBudgetMensuel = new()
    {
        Chart = new Chart
        {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Toolbar = new Toolbar { Show = false },
            Height = "500px"
        },

        Grid = new Grid
        {
            BorderColor = "#444"
        },

        Tooltip = new Tooltip
        {
            Theme = Mode.Dark,
            Shared = true,    // Affiche Crédit ET Débit en même temps dans un seul tooltip
            Intersect = false // Permet de détecter le survol sur toute la colonne X
        },

        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                DataLabels = new PlotOptionsBarDataLabels
                {
                    Position = BarDataLabelPosition.Top
                },

                BorderRadius = 6,
            }
        },

        Yaxis = new List<YAxis>
        {
            new YAxis { Min = 0 }
        },

        Xaxis = new XAxis
        {
            Crosshairs = new AxisCrosshairs
            {
                Fill = new CrosshairsFill
                {
                    Color = "transparent"
                }
            }
        }
    };  
}