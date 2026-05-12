using ApexCharts;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;

namespace Investissement_WebClient.Application.ChartsOptions;

public static class PatrimoineChartsOptions
{
    public static ApexChartOptions<BougieJournaliereCandleChartVM> OptionsBougieJournalierePlusOuMoinsValues = new()
    {
        
        Stroke = new Stroke
        {
            Curve = Curve.Smooth, Width = 3
        },

        Chart = new Chart
        {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Toolbar = new Toolbar { Show = false } ,
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

    public static ApexChartOptions<BougieJournaliereCandleChartVM> OptionsBougieJournaliereValeurPatrimoineSurInvestissementTotal = new()
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

    public static ApexChartOptions<ValeurTotaleParActifVM> OptionsPieActif = new()
    {
        Chart = new Chart
        {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Width = "100%",
            Height = "100%",
        },

        Theme = new Theme
        {
            Palette = PaletteType.Palette2,
        },

        DataLabels = new DataLabels
        {
            Enabled = true,
            DropShadow = new DropShadow { Enabled = true }
        },

        PlotOptions = new PlotOptions
        {
            Pie = new PlotOptionsPie
            {
                DataLabels = new PieDataLabels()
                {
                    MinAngleToShowLabel = 0,
                    Offset = 20
                }
            }
        },

        Title = new Title
        {
            Align = Align.Center,
            Style = new TitleStyle
            {
                FontSize = "16px",
                FontWeight = "bold",
                Color = "#FFFFFF"
            }
        },

        Legend = new Legend
        {
            Show = true,
            Position = LegendPosition.Bottom,
        }
    };
}