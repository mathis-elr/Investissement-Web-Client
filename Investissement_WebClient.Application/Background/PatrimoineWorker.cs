using Investissement_WebClient.Application.Services.Investissement;
using Investissement_WebClient.Application.Services.Patrimoine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Application.Background;

public class PatrimoineWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public PatrimoineWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var patrimoineService = scope.ServiceProvider.GetRequiredService<IPatrimoineService>();
                var dateDernierHistorique = await patrimoineService.GetDateDernierEnregistrement();

                if (dateDernierHistorique == null || (DateTime.Now - dateDernierHistorique.Value) >= TimeSpan.FromHours(1))
                {
                    var investissementService = scope.ServiceProvider.GetRequiredService<IInvestissementService>();

                    try
                    {                 
                        var prixParActif = await investissementService.GetPrixParActif();
                        var valeurPatrimoine = await investissementService.CalculerValeurCourante(prixParActif);
                        var valeurInvestissementTotal = await investissementService.CalculerValeurInvestissementTotal();
                        if (valeurPatrimoine != 0) await patrimoineService.SaveValeurPatrimoine(valeurPatrimoine, valeurInvestissementTotal);
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