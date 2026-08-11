using Investissement_WebClient.Application.Interfaces.APIs;
using Microsoft.Extensions.Options;

namespace Investissement_WebClient.Infrastructure.APIs.LogoDev
{
    public class LogoDevApiService(HttpClient httpClient, 
                                   IOptions<LogoDevApiOptions> options) : ILogoDevApiService
    {
        public HttpClient _httpClient { get; } = httpClient;
        public LogoDevApiOptions _options { get; } = options.Value;

        public async Task<byte[]?> GetLogoByNameAsync(string name)
        {
            var url =
                $"{_options.BaseUri}" +
                $"{_options.SearchByNameEndPoint}" +
                $"{name}" +
                $"{_options.CleeApiKey}" +
                $"{_options.CleeApiValue}";

            try
            {
                return await _httpClient.GetByteArrayAsync(url);
            }
            catch
            {
                return null;
            }
        }
    }
}
