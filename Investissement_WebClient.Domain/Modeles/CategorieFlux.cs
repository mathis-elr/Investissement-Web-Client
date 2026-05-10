namespace Investissement_WebClient.Domain.Modeles;

public class CategorieFlux
{
    public int Id { get; set; }

    public string? MacroCategorie { get; set; }

    public required string MicroCategorie { get; set; }
}