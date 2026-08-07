using Investissement_WebClient.Application.DTO.Patrimoine;
using ApexCharts;

namespace Investissement_WebClient.Web.ChartsOptions
{
    public static class PatrimoineChartsOptions
    {
        public static ApexChartOptions<BougieChartDto> OptionsBougiesPlusValue = new()
        {
            Chart = new Chart
            {
                ForeColor = "#FFFFFF",
                Background = "transparent",
                Toolbar = new Toolbar { Show = false },
                Animations = new Animations
                {
                    Enabled = true,
                    Speed = 100,
                    AnimateGradually = new AnimateGradually
                    {
                        Enabled = false
                    },
                    DynamicAnimation = new DynamicAnimation
                    {
                        Enabled = false
                    },
                },
                Width = "100%",
                Height = "100%",
            },

            Stroke = new Stroke
            {
                Width = 3,
                Curve = Curve.Smooth,
                Colors = new List<string>
                {
                    "goldenrod"
                }
            },

            Grid = new Grid
            {
                BorderColor = "#444"
            },

            Tooltip = new Tooltip
            {
                Theme = Mode.Dark,
                X = new TooltipX
                {
                    Formatter = @"
                        function(value) {
                            return new Date(value)
                                .toLocaleDateString('fr-FR', {
                                    day: 'numeric',
                                    month: 'long',
                                    year: 'numeric'
                                });
                        }"
                }
            },

            Yaxis = new List<YAxis>
            {
                new()
                {
                    Labels = new YAxisLabels
                    {
                        Formatter = @"function(value) {
                            return value.toLocaleString('fr-FR') + ' €'
                        }"
                    }
                }
            },

            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Labels = new XAxisLabels
                {
                    DatetimeUTC = false
                }
            }
        };

        public static ApexChartOptions<PointChartDto> OptionsPointsPlusValue = new()
        {
            Chart = new Chart
            {
                ForeColor = "#FFFFFF",
                Background = "transparent",
                Toolbar = new Toolbar { Show = false },
                Animations = new Animations
                {
                    Enabled = true,
                    Speed = 100,
                    AnimateGradually = new AnimateGradually
                    {
                        Enabled = false
                    },
                    DynamicAnimation = new DynamicAnimation
                    {
                        Enabled = false
                    },
                },
                Width = "100%",
                Height = "100%",
            },

            Stroke = new Stroke
            {
                Width = 3,
                Curve = Curve.Smooth,
                Colors = new List<string>
                {
                    "goldenrod"
                }
            },

            Grid = new Grid
            {
                BorderColor = "#444"
            },

            Tooltip = new Tooltip
            {
                Theme = Mode.Dark,
                X = new TooltipX
                {
                    Formatter = @"
                        function(value) {
                            return new Date(value)
                                .toLocaleDateString('fr-FR', {
                                    day: 'numeric',
                                    month: 'long',
                                    year: 'numeric'
                                });
                        }"
                }
            },

            Yaxis = new List<YAxis>
            {
                new()
                {
                    Labels = new YAxisLabels
                    {
                        Formatter = @"function(value) {
                            return value.toLocaleString('fr-FR') + ' €'
                        }"
                    }
                }
            },

            Xaxis = new XAxis
            {
                Type = XAxisType.Datetime,
                Labels = new XAxisLabels
                {
                    DatetimeUTC = false
                }
            },
        };

        public static ApexChartOptions<BougieChartDto> OptionsBougieJournaliereValeurPatrimoineSurInvestissementTotal = new()
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
                Height = "100%",
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

