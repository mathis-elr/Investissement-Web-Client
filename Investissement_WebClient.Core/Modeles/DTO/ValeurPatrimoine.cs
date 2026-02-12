namespace Investissement_WebClient.Core.Modeles.DTO;

public class ValeurPatrimoine
{
    // La date du point (utilisée pour l'axe X)
    public DateTime Date { get; set; }

    // Prix à l'ouverture de la période
    public decimal Open { get; set; }

    // Prix le plus haut atteint
    public decimal High { get; set; }

    // Prix le plus bas atteint
    public decimal Low { get; set; }

    // Prix à la clôture de la période
    public decimal Close { get; set; }

    // Optionnel : Volume si tu veux l'afficher plus tard
    public decimal Volume { get; set; }
}