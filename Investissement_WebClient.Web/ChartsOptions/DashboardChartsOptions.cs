using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Application.DTO.Profil;
using ApexCharts;

namespace Investissement_WebClient.Web.ChartsOptions
{
    public static class DashboardChartsOptions
    {
        public static ApexChartOptions<ValeurParAnLineChartDto> OptionsValeurParAn = new()
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

        public static ApexChartOptions<ValeurTotaleParActifDto> OptionsPieActif = new()
        {
            Chart = new Chart
            {
                ForeColor = "#FFFFFF",
                Background = "transparent",
                Width = "100%",
                Height = "100%",
                ParentHeightOffset = 0,
                Toolbar = new Toolbar { Show = false }
            },

            Colors = new()
            {
            // Poids lourds (Top allocations) : Doré lumineux -> Ambre riche
                "#F59E0B", // Ambre vif (tranche 1)
                "#D97706", // Doré chaud
                "#B45309", // Bronze soutenu
                "#92400E", // Cuivre sombre

                // Poids intermédiaires : Terres & Taupes chauds contrastés
                "#A88365", // Taupe doré
                "#8C6D53", // Bronze terre
                "#78583E", // Café

                // Petites positions : Ardoises & Gris clairs (très lisibles sur noir)
                "#94A3B8", // Ardoise claire (évite de se perdre dans le fond)
                "#64748B", // Acier
                "#475569", // Ardoise sombre
                "#334155", // Bleu-nuit ardoise
                "#CBD5E1"  // Argent pour le reste / "Autres"
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