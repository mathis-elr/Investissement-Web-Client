namespace Investissement_WebClient.Domain.Modeles;

public class ValeurPatrimoine
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal InvestissementTotal { get; set; }
    
    public decimal Valeur { get; set; }

    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
}