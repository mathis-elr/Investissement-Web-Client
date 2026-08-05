namespace Investissement_WebClient.Infrastructure.APIs.Powens
{
    public class PowensApiOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string BaseUri { get; set; } = string.Empty;

        public string ConnectEndPoint { get; set; } = string.Empty;
        public string TokenEndPoint { get; set; } = string.Empty;
        public string AccountsEndPoint { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;
    }
}


