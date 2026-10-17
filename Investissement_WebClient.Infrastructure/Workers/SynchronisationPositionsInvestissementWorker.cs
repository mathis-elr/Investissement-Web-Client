using Investissement_WebClient.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Infrastructure.Workers
{
    public class SynchronisationPositionsInvestissementWorker(IServiceProvider serviceProvider, IHostEnvironment environment) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IHostEnvironment _environment = environment;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var positionInvestissementService = scope.ServiceProvider.GetRequiredService<IPositionInvestissementService>();

                try
                {
                    if (!_environment.IsDevelopment()) await positionInvestissementService.SynchroniserPositions();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur de synchronisation des positions d'investissement : {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}