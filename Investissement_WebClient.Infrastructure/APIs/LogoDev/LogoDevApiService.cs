using Investissement_WebClient.Application.Interfaces.APIs;
using Microsoft.Extensions.Options;

namespace Investissement_WebClient.Infrastructure.APIs.LogoDev
{
    public class LogoDevApiService(IOptions<LogoDevApiOptions> options) : ILogoDevApiService
    {
        public LogoDevApiOptions _options { get; } = options.Value;

        public string GetUrlLogoByName(string name)
        {
            return $"{_options.BaseUri}" +
                   $"{string.Format(_options.SearchByNameEndPoint, name)}" +
                   $"{_options.TokenKey}" +
                   $"{_options.TokenValue}";
        }
    }
}
