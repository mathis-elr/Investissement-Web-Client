using ApexCharts;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;

namespace Investissement_WebClient.Application.ChartsOptions;

public static class BudgetChartsOptions
{
    public static ApexChartOptions<ValeurParCategorieBarChartVM> OptionsRecapitulatifBudgetMensuel = new()
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

    public static ApexChartOptions<BudgetParMoisLineChartVM> OptionBudgetLineChart = new()
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

        Markers = new Markers
        {
            Size = 4, 
            StrokeWidth = 0,
            Hover = new MarkersHover { Size = 6 }
        },
    };
}