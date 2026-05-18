using ApexCharts;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Profils;

namespace Investissement_WebClient.Application.ChartsOptions
{
    public static class ProfilChartsOptions
    {
        public static ApexChartOptions<ValeurParAnLineChartVM> OptionsValeurParAn = new()
        {

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

        public static ApexChartOptions<InvestissementParMoisVM> OptionsInvestissementParMois = new()
        {
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

        };   
    }
}