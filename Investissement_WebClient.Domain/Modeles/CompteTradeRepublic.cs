namespace Investissement_WebClient.Domain.Modeles
{
    public class CompteTradeRepublic
    {
        public int Id { get; set; }

        public required string NumTelCrypte { get; set; }

        public required string PinCrypte { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; } = null!;
    }
}
