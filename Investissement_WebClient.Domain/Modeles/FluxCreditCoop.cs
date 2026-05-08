namespace Investissement_WebClient.Domain.Modeles;

public class FluxCreditCoop
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal Valeur { get; set; }
    
    public required string LibelleRecu { get; set; }

    public int? IdCategorie { get; set; }
    public CategorieFlux? Categorie { get; set; }
}