namespace Investissement_WebClient.Application.Interfaces.APIs
{
    public interface ILogoDevApiService
    {
        Task<byte[]?> GetLogoByNameAsync(string name);
    }
}
