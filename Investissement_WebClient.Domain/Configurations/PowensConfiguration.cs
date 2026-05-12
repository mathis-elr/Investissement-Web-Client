namespace Investissement_WebClient.Domain.Configurations;

public static class PowensConfiguration
{
    public static string ClientId { get; set; }
    
    public static string ClientSecret { get; set; }

    public static string BaseUrl { get; set; }

    public static string RedirectUri { get; set; }

    public static string ConnectUrl { get; set; }

    public static string GetConnexionUrl()
    {
        var fullConnectUrl = new Uri(new Uri(BaseUrl), ConnectUrl).ToString();

        var encodedRedirect = Uri.EscapeDataString(RedirectUri);

        return $"{fullConnectUrl}?client_id={ClientId}&redirect_uri={encodedRedirect}";
    }
}