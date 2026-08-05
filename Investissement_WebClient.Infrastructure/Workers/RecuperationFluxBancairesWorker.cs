using Investissement_WebClient.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Infrastructure.Workers
{
    public class RecuperationFluxBancairesWorker(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Day > 5)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var fluxBancaireService = scope.ServiceProvider.GetRequiredService<IFluxBancaireService>();
                    await fluxBancaireService.VerifierEtSynchroniserFluxBancairesAsync();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
