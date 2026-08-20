using Investissement_WebClient.Application.Interfaces.APIs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Infrastructure.Workers
{
    public class RecuperationSoldeComptesWorker(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var powensApiService = scope.ServiceProvider.GetRequiredService<IPowensApiService>();

                try
                {
                    await powensApiService.SynchroniserSoldeComptes();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur de synchronisation des soldes de comptes : {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
