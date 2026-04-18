using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Data.Background;

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
                var investissementService = scope.ServiceProvider.GetRequiredService<IInvestissementService>();
                var patrimoineService = scope.ServiceProvider.GetRequiredService<IPatrimoineService>();
                
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
            
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}