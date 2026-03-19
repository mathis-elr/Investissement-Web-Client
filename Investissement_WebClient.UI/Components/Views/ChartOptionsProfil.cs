using ApexCharts;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.Views
{
    public static class ChartOptionsProfil
    {
        public static ApexChartOptions<ValeurParAn> OptionsValeurParAn= new()
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
