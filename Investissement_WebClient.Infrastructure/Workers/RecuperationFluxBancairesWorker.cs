using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Interfaces.APIs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Infrastructure.Workers
{
    public class RecuperationFluxBancairesWorker(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly SemaphoreSlim _signalSyncImmediate = new(0); // Permet de réveiller le worker instantanément
        private int _dernierUserIdCible;

        public void DeclencherSynchronisationImmediate(int userId)
        {
            _dernierUserIdCible = userId;
            _signalSyncImmediate.Release(); // Libère le sémaphore pour lancer le traitement tout de suite
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    bool isJourOk = DateTime.Now.Day >= 5;
                    bool signaled = await _signalSyncImmediate.WaitAsync(TimeSpan.FromDays(1), stoppingToken);

                    if (isJourOk || signaled)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var powensApiService = scope.ServiceProvider.GetRequiredService<IPowensApiService>();

                        if (signaled)
                            await ExecuterBoucleRatissageAsync(powensApiService, stoppingToken);
                        else
                            await powensApiService.VerifierEtSynchroniserFluxBancairesAsync();
                    }

                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur dans le worker de synchro : {ex.Message}");
                }
            }
        }

        private async Task ExecuterBoucleRatissageAsync(IPowensApiService powensApiService, CancellationToken stoppingToken)
        {
            for (int i = 0; i < 15; i++)
            {
                if (stoppingToken.IsCancellationRequested) return;

                await Task.Delay(2000, stoppingToken);

                await powensApiService.VerifierEtSynchroniserFluxBancairesAsync();

                using var scope = _serviceProvider.CreateScope();
                var fluxBancaireService = scope.ServiceProvider.GetRequiredService<IFluxBancaireService>();

                var flux = await fluxBancaireService.GetFluxBancaire(_dernierUserIdCible);
                if (flux.Count != 0) break;
            }
        }
    }
}
