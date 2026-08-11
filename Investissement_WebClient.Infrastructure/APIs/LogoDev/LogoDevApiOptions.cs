namespace Investissement_WebClient.Infrastructure.APIs.LogoDev
{
    public class LogoDevApiOptions
    {
        public string CleeApiKey { get; set; } = string.Empty;

        public string CleeApiValue { get; set; } = string.Empty;

        public string BaseUri { get; set; } = string.Empty;

        public string SearchByNameEndPoint { get; set; } = string.Empty;

        public string SearchByIsinEndPoint { get; set; } = string.Empty;

        public string SearchByTickerEndPoint { get; set; } = string.Empty;
    }
}
