using Investissement_WebClient.Application.DTO.FluxBancaires;
using ApexCharts;

namespace Investissement_WebClient.Web.ChartsOptions
{
    public static class BudgetChartsOptions
    {
        public static ApexChartOptions<ValeurParCategorieBarChartDto> OptionsRecapitulatifBudgetMensuel = new()
        {
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Labels = new YAxisLabels
                    {
                        Formatter = @"function(val) { return Math.round(val).toLocaleString('fr-FR') + ' €'; }"
                    }
                }
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

        public static ApexChartOptions<BudgetParMoisLineChartDto> OptionBudgetLineChart = new()
        {
            Yaxis = new List<YAxis>
            {
                new YAxis
                {
                    Labels = new YAxisLabels
                    {
                        Formatter = @"function(val) { return Math.round(val).toLocaleString('fr-FR') + ' €'; }"
                    }
                }
            },

            Xaxis = new XAxis
            {
                Type = XAxisType.Category,
                TickPlacement = TickPlacement.Between,
                Labels = new XAxisLabels
                {
                    Rotate = 0,
                    Formatter = @"function(val) { 
                        if (!val) return '';
                        var d = new Date(val);
                        if (isNaN(d.getTime())) return val;
                        return d.toLocaleDateString('fr-FR', { month: 'short', year: '2-digit' });
                    }"
                }
            },

            Theme = new Theme
            {
                Mode = Mode.Dark,
            },

            Colors =  new List<string> { "#22c55e", "#3b82f6", "#ef4444", "#eab308" },

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

            Legend = new Legend
            {
                Show = false
            }
        };
    }
}