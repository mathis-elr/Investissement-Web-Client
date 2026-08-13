using ApexCharts;
using Investissement_WebClient.Application.DTO.FluxInvestissements;

namespace Investissement_WebClient.Web.ChartsOptions
{
    public static class InvestissementChartsOptions
    {
        public static ApexChartOptions<InvestissementParMoisDto> OptionsInvestissementParMois = new()
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

            Colors = new List<string> { "goldenrod" },

            Stroke = new Stroke
            {
                Curve = Curve.Smooth,
                Width = 3,
                LineCap = LineCap.Round,
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
                Colors = new List<string> { "goldenrod" },
                StrokeWidth = 0,
            },
        };
    }
}
