using ApexCharts;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Application.Charts
{
    public static class ChartOptionsProfil
    {
        public static ApexChartOptions<ValeurParAnVM> OptionsValeurParAn = new()
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
}