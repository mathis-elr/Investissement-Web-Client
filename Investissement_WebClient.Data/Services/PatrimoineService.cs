using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class PatrimoineService : IPatrimoineService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly ITransactionService _transactionService;
    private readonly IYahooDataService _yahooDataService;

    public PatrimoineService(IDbContextFactory<InvestissementDbContext> dbContext , ITransactionService transactionService, IYahooDataService yahooDataService)
    {
        _dbFactory = dbContext;
        _transactionService = transactionService;
        _yahooDataService = yahooDataService;
    }

    public async Task<double> CalculerValeurPatrimoineCourante()
    {
        var detailsActifs = await _transactionService.GetDetailsActif();
        
        var symboles = detailsActifs.Select(d => d.SymboleActif).ToList();
        
        var prixParActif = await _yahooDataService.GetPrixActuelAsync(symboles);

        return detailsActifs.Sum(a => a.QuantiteDetenue * prixParActif[a.SymboleActif]);
    }

    public async Task<VariationsDto> GetVariations(double valeurActuelle)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var unAnsAvantAjd = DateTime.Now.AddYears(-1);
        
        var historiqueAnnee = await context.HistoriquePatrimoine
            .Where(h => h.Date >= unAnsAvantAjd)
            .OrderByDescending(h => h.Date)
            .ToListAsync();

        return new VariationsDto
        {
            VariationPrix24H = CalculVariationPeriode(valeurActuelle, historiqueAnnee, 1),
            VariationPrix7J = CalculVariationPeriode(valeurActuelle, historiqueAnnee, 7),
            VariationPrix1M = CalculVariationPeriode(valeurActuelle, historiqueAnnee, 30),
            VariationPrix1A = CalculVariationPeriode(valeurActuelle, historiqueAnnee, 365),
        };
    }

    private double CalculVariationPeriode(double valeurActuelle, List<HistoriquePatrimoine> historique, int periode)
    {
        DateTime dateDebutPeriode = DateTime.Now.AddDays(-periode);

        var ancienneValeur = historique
            .Where(h => h.Date >= dateDebutPeriode)
            .OrderBy(h => h.Date)
            .Select(h => h.Valeur)
            .FirstOrDefault();
        
        return Math.Round(((valeurActuelle - ancienneValeur) / ancienneValeur) * 100, 2);
    }
}