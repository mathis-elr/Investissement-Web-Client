namespace Investissement_WebClient.Domain.Modeles;

public class FluxBancaire
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal Valeur { get; set; }
    
    public required string Libelle { get; set; }

    public int? IdCategorie { get; set; }
    public CategorieFlux Categorie { get; set; } = null!;

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
}