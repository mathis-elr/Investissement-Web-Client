namespace Investissement_WebClient.Domain.Configurations
{
    public static class TradeRepublicApiConfiguration
    {
        public static string BaseUri { get; set; } = null!;
        public static string RequestSmsEndPoint { get; set; } = null!;
        public static string ConfirmSmsEndPoint { get; set; } = null!;
        public static string DatasEndPoint { get; set; } = null!;

        public static string CleeApiKey { get; set; } = null!;
        public static string CleeApiValue { get; set; } = null!;

        public static string NumTelKey { get; set; } = null!;
        public static string NumTelValue { get; set; } = null!;

        public static string PinKey { get; set; } = null!;
        public static string PinValue { get; set; } = null!;

        public static string DernierIdEnregistreKey { get; set; } = null!;
    }
}
