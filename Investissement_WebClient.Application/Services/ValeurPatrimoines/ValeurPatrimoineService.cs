using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.ValeurPatrimoines;

public class ValeurPatrimoineService : IValeurPatrimoineService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

    public ValeurPatrimoineService(IDbContextFactory<InvestissementDbContext> dbContext)
    {
        _dbFactory = dbContext;
    }

    public async Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal, int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var newValeurPatrimoine = new ValeurPatrimoine
        {
            Date = DateTime.Now,
            Valeur = valeurPatrimoine,
            InvestissementTotal = valeurInvestissementTotal,
            UtilisateurId = userId
        };

        context.ValeurPatrimoine.Add(newValeurPatrimoine);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<VariationDto>> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal, int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var unAnsAvantAjd = DateTime.Now.AddYears(-1);
        
        var historiqueAnnee = await context.ValeurPatrimoine
            .Where(h => h.UtilisateurId == userId)
            .Where(h => h.Date >= unAnsAvantAjd)
            .OrderByDescending(h => h.Date)
            .ToListAsync();

        return new List<VariationDto>
        {
            new() {Label = "24H", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 1)},
            new() {Label = "7J", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 7)},
            new() {Label = "1M", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 30)},
            new() {Label = "1A", Valeur = CalculVariationPeriode(valeurActuelle, valeurInvestissementTotal, historiqueAnnee, 365)}
        };
    }

    public async Task<DateTime?> GetDateDernierEnregistrement(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.ValeurPatrimoine
                               .Where(h => h.UtilisateurId == userId)
                               .OrderByDescending(h => h.Date)
                               .Select(f  => f.Date)
                               .FirstOrDefaultAsync();
    }

    private decimal CalculVariationPeriode(decimal valeurActuelle, decimal valeurInvestissementTotal, List<ValeurPatrimoine> historique, int periode)
    {
        DateTime dateDebutPeriode = DateTime.Now.AddDays(-periode);
        
        var ancienProfit = historique
            .Where(h => h.Date >= dateDebutPeriode)
            .OrderBy(h => h.Date)
            .Select(h =>  h.Valeur - h.InvestissementTotal)
            .FirstOrDefault();
        
        decimal nouveauProfit = valeurActuelle - valeurInvestissementTotal;
        decimal variation = (nouveauProfit - ancienProfit) / valeurInvestissementTotal;
        return variation;
    }

    public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresPlusOuMoinsValues(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var donneesGroupes = await context.ValeurPatrimoine
            .Where(h => h.UtilisateurId == userId)
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
        
        return donneesGroupes.Select(dg => {

            decimal valeurOuverture = dg.DonneesJour.FirstOrDefault()?.Valeur ?? 0;
            decimal valeurFermeture = dg.DonneesJour.LastOrDefault()?.Valeur ?? 0;
            decimal investissementOuverture = dg.DonneesJour.FirstOrDefault()?.InvestissementTotal ?? 0;
            decimal investissementFermeture = dg.DonneesJour.LastOrDefault()?.InvestissementTotal ?? 0;

            return new BougieJournaliereCandleChartVM
            {
                Date = dg.Date,
                Ouverture = Math.Round(valeurOuverture - investissementOuverture, 2), //haut de la pile = date la plus ancienne
                Fermeture = Math.Round(valeurFermeture - investissementFermeture, 2),
                Haut = Math.Round(dg.Max, 2),
                Bas = Math.Round(dg.Min, 2),
            };
        }).ToList();
    }

    public async Task<IEnumerable<BougieJournaliereCandleChartVM>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(int userId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var data = await context.ValeurPatrimoine
            .Where(h => h.UtilisateurId == userId)
            .GroupBy(hp => hp.Date.Date)
            .Select(hp => new
            {
                Date = hp.Key,
                MaxValeur = hp.Max(t => t.Valeur),
                MinValeur = hp.Min(t => t.Valeur),
                DonnesParJour = hp.OrderBy(hp => hp.Date).Select(t => new
                {
                    t.Valeur,
                    t.InvestissementTotal
                }),
                InvestissementTotal = hp.Max(testc => testc.InvestissementTotal)
        }).OrderBy(hp => hp.Date)
          .ToListAsync();

        return data.Select(t => new BougieJournaliereCandleChartVM
        {
            Date = t.Date,
            Ouverture = t.DonnesParJour.FirstOrDefault()?.Valeur ?? 0,
            Fermeture = t.DonnesParJour.LastOrDefault()?.Valeur ?? 0,
            Bas = t.MinValeur,
            Haut = t.MaxValeur,
            InvestissementTotal = t.InvestissementTotal,
        }).ToList();
    }
}