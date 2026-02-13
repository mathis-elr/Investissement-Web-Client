using ApexCharts;
using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.UI.Components;

public static class ChartsOptions
{
    public static ApexChartOptions<ValeurPatrimoine> Options = new()
    {
        
        Stroke = new Stroke
        {
            Curve = Curve.Smooth, Width = 3
        },

        Chart = new Chart {
            ForeColor = "#FFFFFF",
            Background = "transparent",
            Toolbar = new Toolbar { Show = false } ,
            Width = "100%",
            Height = "500px"
        },

        Grid = new Grid {
            BorderColor = "#444" 
        }
    };
}