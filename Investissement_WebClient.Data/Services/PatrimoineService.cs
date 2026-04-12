using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services;

public class PatrimoineService : IPatrimoineService
{
    private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
    private readonly IYahooDataService _yahooDataService;

    public PatrimoineService(IDbContextFactory<InvestissementDbContext> dbContext, IYahooDataService yahooDataService, IInvestissementService investissementService)
    {
        _dbFactory = dbContext;
        _yahooDataService = yahooDataService;
    }

    public async Task SaveValeurPatrimoine(decimal valeurPatrimoine, decimal valeurInvestissementTotal)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var newValeurPatrimoine = new HistoriquePatrimoine
        {
            Date = DateTime.Now,
            Valeur = valeurPatrimoine,
            InvestissementTotal = valeurInvestissementTotal
        };

        context.HistoriquePatrimoine.Add(newValeurPatrimoine);
        await context.SaveChangesAsync();
    }

    public async Task<VariationsDto> GetVariations(decimal valeurActuelle, decimal valeurInvestissementTotal)
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

    private decimal CalculVariationPeriode(decimal valeurActuelle, decimal valeurInvestissementTotal, List<HistoriquePatrimoine> historique, int periode)
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

    public async Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieresPlusOuMoinsValues()
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
        
        return donneesGroupes.Select(dg => {

            decimal valeurOuverture = dg.DonneesJour.FirstOrDefault()?.Valeur ?? 0;
            decimal valeurFermeture = dg.DonneesJour.LastOrDefault()?.Valeur ?? 0;
            decimal investissementOuverture = dg.DonneesJour.FirstOrDefault()?.InvestissementTotal ?? 0;
            decimal investissementFermeture = dg.DonneesJour.LastOrDefault()?.InvestissementTotal ?? 0;

            return new BougieJournaliere
            {
                Date = dg.Date,
                Ouverture = Math.Round(valeurOuverture - investissementOuverture, 2), //haut de la pile = date la plus ancienne
                Fermeture = Math.Round(valeurFermeture - investissementFermeture, 2),
                Haut = Math.Round(dg.Max, 2),
                Bas = Math.Round(dg.Min, 2),
            };
        }).ToList();
    }

    //public async Task<IEnumerable<ProportionActif>> GetProportionParActifInvestit(decimal valeurPatrimoineCourant)
    //{
    //    await using var context = await _dbFactory.CreateDbContextAsync();

    //    IEnumerable<DetailsActifDto> detailsActifs = await _actifService.GetDetailsActif();
    //    IEnumerable<string> symboles = detailsActifs.Select(d => d.SymboleActif).ToList();
    //    Dictionary<string, decimal> prixParActif = await _yahooDataService.GetPrixActuelAsync(symboles);

    //    var data = await context.Transactions
    //        .GroupBy(t => new { t.Actif.Nom, t.Actif.Symbole})
    //        .Select(groupe => new
    //        {
    //            Actif = groupe.Key.Nom,
    //            Symbole = groupe.Key.Symbole,
    //            QuantiteTotale = groupe.Sum(t => t.Quantite)
    //        })
    //        .ToListAsync();

    //    return data.Select(t => new ProportionActif
    //        {
    //            Actif = t.Actif,
    //            Proportion = Math.Round(t.QuantiteTotale * (prixParActif.TryGetValue(t.Symbole, out decimal value) ? value : 0)/ valeurPatrimoineCourant, 2)*100,
    //        }).ToList();
    //}

    //public async Task<IEnumerable<ProportionTypeActif>> GetProportionParTypeActifInvestit(decimal valeurPatrimoineCourant)
    //{
    //    await using var context = await _dbFactory.CreateDbContextAsync();

    //    IEnumerable<DetailsActifDto> detailsActifs = await _actifService.GetDetailsActif();
    //    IEnumerable<string> symboles = detailsActifs.Select(d => d.SymboleActif).ToList();
    //    Dictionary<string, decimal> prixParActif = await _yahooDataService.GetPrixActuelAsync(symboles);

    //    var data = await context.Transactions
    //        .GroupBy(t => new { t.Actif.Type, t.Actif.Symbole })
    //        .Select(t => new
    //        {
    //            Type = t.Key.Type,
    //            Symbole = t.Key.Symbole,
    //            QuantiteTotale = t.Sum(t => t.Quantite)
    //        })
    //        .ToListAsync();

    //    return data.Select(t => new ProportionTypeActif
    //        {
    //            Type = t.Type,
    //            Proportion = (decimal)(Math.Round(t.QuantiteTotale * (prixParActif.TryGetValue(t.Symbole, out decimal value) ? value : 0) / valeurPatrimoineCourant, 2) * 100),
    //        }).ToList();
    //}

    public async Task<IEnumerable<BougieJournaliere>> GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var data = await context.HistoriquePatrimoine.GroupBy(hp => hp.Date.Date).Select(hp => new
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

        return data.Select(t => new BougieJournaliere
        {
            Date = t.Date,
            Ouverture = t.DonnesParJour.FirstOrDefault()?.Valeur ?? 0,
            Fermeture = t.DonnesParJour.LastOrDefault()?.Valeur ?? 0,
            Bas = t.MinValeur,
            Haut = t.MaxValeur,
            InvestissementTotal = t.InvestissementTotal,
        }).ToList();
    }


    public async Task DeleteHistoriquePatrimoinePeriode(DateTime dateDepart)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        var enregistrementsASupprimer = context.HistoriquePatrimoine.Where(hp => hp.Date >= dateDepart && hp.Date <= DateTime.Now).ToList();
        
        context.HistoriquePatrimoine.RemoveRange(enregistrementsASupprimer);
        await context.SaveChangesAsync();
    }
}