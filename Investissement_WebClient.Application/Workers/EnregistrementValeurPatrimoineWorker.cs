using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.ValeurPatrimoines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Application.Workers;

public class EnregistrementValeurPatrimoineWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public EnregistrementValeurPatrimoineWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var valeurPatrimoineService = scope.ServiceProvider.GetRequiredService<IValeurPatrimoineService>();
                var dateDernierHistorique = await valeurPatrimoineService.GetDateDernierEnregistrement();

                if (dateDernierHistorique == null || (DateTime.Now - dateDernierHistorique.Value) >= TimeSpan.FromHours(1))
                {
                    var fluxInvestissementService = scope.ServiceProvider.GetRequiredService<IFluxInvestissementService>();

                    try
                    {                 
                        var prixParActif = await fluxInvestissementService.GetPrixParActif();
                        var valeurPatrimoine = await fluxInvestissementService.CalculerValeurCourante(prixParActif);
                        var valeurInvestissementTotal = await fluxInvestissementService.CalculerValeurInvestissementTotal();
                        if (valeurPatrimoine != 0) await valeurPatrimoineService.SaveValeurPatrimoine(valeurPatrimoine, valeurInvestissementTotal);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur d'enregistrement d'un historique de patrimoine : {ex.Message}");
                    }
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}