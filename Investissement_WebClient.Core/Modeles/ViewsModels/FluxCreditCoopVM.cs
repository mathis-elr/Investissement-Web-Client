namespace Investissement_WebClient.Core.Modeles.ViewsModels;

public class FluxCreditCoopVM
{

    public int Id { get; set; }
    
    public DateTime Date { get; set; }

    public decimal Valeur { get; set; }
    
    public string LibelleRecu { get; set; }
    
    public int IdCategorie { get; set; }
}