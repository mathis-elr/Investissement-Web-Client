using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class PatrimoineService : IPatrimoineService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly IInvestirService _investirService;
    private readonly IYahooDataService _yahooDataService;

    public PatrimoineService(IDbContextFactory<InvestissementDbContext> dbContext , IInvestirService investirService, IYahooDataService yahooDataService)
    {
        _dbFactory = dbContext;
        _investirService = investirService;
        _yahooDataService = yahooDataService;
    }

    public async Task<double> CalculerValeurPatrimoineCourante()
    {
        var detailsActifs = await _investirService.GetDetailsActif();
        
        var symboles = detailsActifs.Select(d => d.SymboleActif).ToList();
        
        var prixParActif = await _yahooDataService.GetPrixActuelAsync(symboles);

        return detailsActifs.Sum(a => a.QuantiteDetenue * prixParActif[a.SymboleActif]);
    }

    public async Task<double> CalculerValeurInvestissementTotal()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        return await context.Transactions
            .SumAsync(t => t.Quantite * t.Prix);
    }

    public async Task<VariationsDto> GetVariations(double valeurActuelle, double valeurInvestissementTotal)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var unAnsAvantAjd = DateTime.Now.AddYears(-1);
        
        var historiqueAnnee = await context.HistoriquePatrimoine
            .Where(h => h.Date >= unAnsAvantAjd)
            .OrderByDescending(h => h.Date)
            .ToListAsync();

        return new VariationsDto
        {
            VariationPrix24H = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 1),
            VariationPrix7J = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 7),
            VariationPrix1M = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 30),
            VariationPrix1A = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 365),
        };
    }

    private double CalculVariationPeriode(double valeurActuelle, double valeurInvestissementTotal, List<HistoriquePatrimoine> historique, int periode)
    {
        DateTime dateDebutPeriode = DateTime.Now.AddDays(-periode);
        
        var ancienProfit = historique
            .Where(h => h.Date >= dateDebutPeriode)
            .OrderBy(h => h.Date)
            .Select(h =>  h.Valeur - h.InvestissementTotal)
            .FirstOrDefault();
        
        double nouveauProfit = valeurActuelle - valeurInvestissementTotal;
        double variation = (nouveauProfit - ancienProfit) / valeurInvestissementTotal;
        return variation;
    }

    public async Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieres()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var donneesGroupes = await context.HistoriquePatrimoine
            .GroupBy(hp => hp.Date.Date)
            .Select(d => new
            {
                Date = d.Key,
                Max = d.Max(max => max.Valeur - max.InvestissementTotal),
                Min = d.Min(min => min.Valeur - min.InvestissementTotal),
                DonneesJour = d.OrderBy(hp => hp.Date).Select(hp => new // on range de la première heure de la journée à la dernière
                {
                    hp.Valeur, 
                    hp.InvestissementTotal 
                    
                })
            })
            .OrderBy(dg => dg.Date) //on range les jours du plus ancien au plus recent 
            .ToListAsync();
        
        return donneesGroupes.Select(dg => new BougieJournaliere
        {
            Date = dg.Date,
            Ouverture = (decimal)Math.Round(dg.DonneesJour.First().Valeur - dg.DonneesJour.First().InvestissementTotal,2), //haut de la pile = date la plus ancienne
            Fermeture = (decimal)Math.Round(dg.DonneesJour.Last().Valeur - dg.DonneesJour.Last().InvestissementTotal, 2),
            Haut = (decimal)Math.Round(dg.Max, 2),
            Bas = (decimal)Math.Round(dg.Min, 2)
        }).ToList();
    }
}