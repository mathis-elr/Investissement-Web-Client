namespace Investissement_WebClient.Infrastructure.APIs.TradeRepublic
{
    public class TradeRepublicApiOptions
    {
        public string BaseUri { get; set; } = string.Empty;
        public string RequestSmsEndPoint { get; set; } = string.Empty;
        public string ConfirmSmsEndPoint { get; set; } = string.Empty;
        public string DatasEndPoint { get; set; } = string.Empty;

        public string CleeApiKey { get; set; } = string.Empty;
        public string CleeApiValue { get; set; } = string.Empty;

        public string NumTelKey { get; set; } = string.Empty;
        public string PinKey { get; set; } = string.Empty;

        public string DernierIdEnregistreKey { get; set; } = string.Empty;

        public string LogoBaseUrl { get; set; } = string.Empty;
    }
}
