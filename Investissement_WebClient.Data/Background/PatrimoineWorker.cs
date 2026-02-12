using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Investissement_WebClient.Data.Background;

public class PatrimoineWorker : BackgroundService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly IServiceProvider _serviceProvider;

    public PatrimoineWorker(IDbContextFactory<InvestissementDbContext> dbContext,  IServiceProvider serviceProvider)
    {
        _dbFactory = dbContext;
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
                    await SaveValeurPatrimoine(valeurPatrimoine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur Worker Patrimoine : {ex.Message}");
                }
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task SaveValeurPatrimoine(double valeur)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var newValeurPatrimoine = new HistoriquePatrimoine
        {
            Date = DateTime.Now,
            Valeur = valeur,
        };
        
        context.HistoriquePatrimoine.Add(newValeurPatrimoine);
        await context.SaveChangesAsync();
    }
}