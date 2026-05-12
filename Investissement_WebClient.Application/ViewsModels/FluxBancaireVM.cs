namespace Investissement_WebClient.Application.ViewsModels;

public class FluxBancaireVM
{

    public int Id { get; set; }
    
    public DateTime Date { get; set; }

    public decimal Valeur { get; set; }
    
    public string LibelleRecu { get; set; }
    
    public int IdCategorie { get; set; }
}