using Investissement_WebClient.Core.InterfacesServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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
                var patrimoineService = scope.ServiceProvider.GetRequiredService<IPatrimoineService>();
                
                try 
                {
                    var valeurPatrimoine = await patrimoineService.CalculerValeurPatrimoineCourante();
                    var valeurInvestissementTotal = await patrimoineService.CalculerValeurInvestissementTotal();
                    if (valeurPatrimoine != 0) await patrimoineService.SaveValeurPatrimoine(valeurPatrimoine, valeurInvestissementTotal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur d'enregistrement d'un historique de patrimoine : {ex.Message}");
                }
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}