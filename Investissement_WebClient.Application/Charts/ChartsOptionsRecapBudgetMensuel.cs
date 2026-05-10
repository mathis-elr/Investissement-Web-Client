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
            Width = "100%",
            Height = "100%"
        },

        Grid = new Grid
        {
            BorderColor = "#444"
        },

        Tooltip = new Tooltip
        {
            Theme = Mode.Dark,
            Intersect = false,
            Shared = false,
            Marker = new TooltipMarker { Show = false }
        },

        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                DataLabels = new PlotOptionsBarDataLabels
                {
                    Position = BarDataLabelPosition.Center
                },

                BorderRadius = 7
            },
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

    public static ApexChartOptions<BudgetCategorieParMoisVM> OptionBudgetLineChart = new()
    {
        Theme = new Theme
        {
            Mode = Mode.Dark,
            Palette = PaletteType.Palette1
        },

        Stroke = new Stroke
        {
            Curve = Curve.Smooth,
            Width = 3
        },

        Chart = new Chart
        {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Toolbar = new Toolbar { Show = false },
            Width = "100%",
            Height = "100%"
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