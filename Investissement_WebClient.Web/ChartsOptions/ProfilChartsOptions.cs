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

            Markers = new Markers
            {
                Colors = new List<string> { "goldenrod" },
                StrokeWidth = 0,
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

            Colors = new()
            {
                "#DAA520", // goldenrod
                "#C6951E",
                "#B7841C",
                "#A87419",
                "#986616",

                "#7A6A53",
                "#696969",
                "#5A5A5A",
                "#4A4A4A",

                "#8E8E8E",
                "#777777",
                "#626262"
            },
            Legend = new Legend
            {
                Position = LegendPosition.Right
            },

            Responsive = new List<Responsive<ValeurTotaleParActifDto>> 
            {
                new Responsive<ValeurTotaleParActifDto> 
                {
                    Breakpoint = 1000, 
                    Options = new ApexChartOptions<ValeurTotaleParActifDto>
                    {
                        Legend = new Legend
                        {
                            Position = LegendPosition.Bottom
                        }
                    }
                }
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