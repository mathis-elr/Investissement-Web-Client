using Investissement_WebClient.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Infrastructure.Workers
{
    public class RecuperationFluxBancairesWorker(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private int _dernierUserIdCible;
        private readonly SemaphoreSlim _signalSyncImmediate = new(0); // Permet de réveiller le worker instantanément

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
                        var fluxBancaireService = scope.ServiceProvider.GetRequiredService<IFluxBancaireService>();

                        if (signaled)
                            await ExecuterBoucleRatissageAsync(fluxBancaireService, stoppingToken);
                        else
                            await fluxBancaireService.VerifierEtSynchroniserFluxBancairesAsync();
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

        private async Task ExecuterBoucleRatissageAsync(IFluxBancaireService fluxBancaireService, CancellationToken stoppingToken)
        {
            for (int i = 0; i < 15; i++)
            {
                if (stoppingToken.IsCancellationRequested) return;

                await Task.Delay(2000, stoppingToken);

                await fluxBancaireService.VerifierEtSynchroniserFluxBancairesAsync();

                var flux = await fluxBancaireService.GetFluxBancaire(_dernierUserIdCible);
                if (flux.Count != 0) break;
            }
        }
    }
}


//using Investissement_WebClient.Application.Interfaces.Services;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace Investissement_WebClient.Infrastructure.Workers
//{
//    public class RecuperationFluxBancairesWorker(IServiceProvider serviceProvider) : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider = serviceProvider;

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                if (DateTime.Now.Day >= 5)
//                {
//                    using var scope = _serviceProvider.CreateScope();
//                    var fluxBancaireService = scope.ServiceProvider.GetRequiredService<IFluxBancaireService>();
//                    await fluxBancaireService.VerifierEtSynchroniserFluxBancairesAsync();
//                }

//                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
//            }
//        }
//    }
//}
