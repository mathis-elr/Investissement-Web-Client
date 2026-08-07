using ApexCharts;
using Investissement_WebClient.Application.DTO.FluxInvestissements;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Application.DTO.Profil;

namespace Investissement_WebClient.Web.ChartsOptions
{
    public static class ProfilChartsOptions
    {
        public static ApexChartOptions<ValeurParAnLineChartDto> OptionsValeurParAn = new()
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

        public static ApexChartOptions<InvestissementParMoisDto> OptionsInvestissementParMois = new()
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

        public static ApexChartOptions<ValeurTotaleParActifDto> OptionsPieActif = new()
        {
            Chart = new Chart
            {
                ForeColor = "#FFFFFF",
                Background = "transparent",
                Width = "100%",
                Height = "100%",
            },

            Colors = Enumerable.Range(1, 20)
                .Select(i => $"hsl({(i * 360 / 20)}, 65%, 55%)")
                .ToList(),

            Legend = new Legend
            {
                Show = true,
                Position = LegendPosition.Right,
            },

            DataLabels = new DataLabels
            {
                DropShadow = new DropShadow { Enabled = false },
                Style = new DataLabelsStyle
                {
                    Colors = new List<string>
                    {
                        "#000000"
                    },
                    FontWeight = "bold"
                }
            }
        }; 
    }
}