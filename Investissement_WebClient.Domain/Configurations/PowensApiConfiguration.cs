namespace Investissement_WebClient.Domain.Configurations;

public static class PowensApiConfiguration
{
    public static string ClientId { get; set; } = null!;

    public static string ClientSecret { get; set; } = null!;

    public static string BaseUrl { get; set; } = null!;

    public static string RedirectUri { get; set; } = null!;

    public static string ConnectUrl { get; set; } = null!;

    public static string GetConnexionUrl()
    {
        var fullConnectUrl = new Uri(new Uri(BaseUrl), ConnectUrl).ToString();

        var encodedRedirect = Uri.EscapeDataString(RedirectUri);

        return $"{fullConnectUrl}?client_id={ClientId}&redirect_uri={encodedRedirect}";
    }
}