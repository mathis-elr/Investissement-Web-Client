using Investissement_WebClient.Application.DTO.FluxInvestissements;
using ApexCharts;

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

            Xaxis = new XAxis
            {
                Type = XAxisType.Category,
                TickPlacement = TickPlacement.On,
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

            Colors = new List<string> { "goldenrod" },

            Stroke = new Stroke
            {
                Curve = Curve.Smooth,
                Width = 3,
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

            Legend = new Legend
            {
                Show = false
            }
        };
    }
}
