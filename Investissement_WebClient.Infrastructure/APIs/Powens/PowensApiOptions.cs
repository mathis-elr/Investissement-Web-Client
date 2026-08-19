namespace Investissement_WebClient.Infrastructure.APIs.Powens
{
    public class PowensApiOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string BaseUri { get; set; } = string.Empty;

        public string NouvelUtilisateurEndPoint { get; set; } = string.Empty;
        public string CodeByTokenEndPoint { get; set; } = string.Empty;
        public string ConnectEndPoint { get; set; } = string.Empty;
        public string AccountsEndPoint { get; set; } = string.Empty;
        public string ConnectionsEndPoint { get; set; } = string.Empty;
        public string AccountsConnectionEndPoint { get; set; } = string.Empty;
        public string ConnectorEndPoint { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;
    }
}


