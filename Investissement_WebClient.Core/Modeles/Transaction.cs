namespace Investissement_WebClient.Core.Modeles;

public class Transaction
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Type { get; set; }

    public string Actif {  get; set; }

    public string ISIN { get; set; }

    public string Ticker { get; set; }

    public decimal Prix { get; set; }

    public decimal Quantite { get; set; }
    
    public decimal Frais { get; set; }

    public decimal Total { get; set; }
}