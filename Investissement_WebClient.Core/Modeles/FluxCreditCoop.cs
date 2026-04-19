namespace Investissement_WebClient.Core.Modeles;

public class FluxCreditCoop
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal Valeur { get; set; }
    
    public string LibelleRecu { get; set; }
    
    public string? LibelleSupplementaire { get; set; }
    
    public CategorieFlux? Categorie { get; set; }
}