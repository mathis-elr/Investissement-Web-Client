using System.Runtime.InteropServices.JavaScript;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.Graphiques;
using Investissement_WebClient.Core.Modeles.ViewsModels;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Data.Services
{
    public class InvestissementService : IInvestissementService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
        private readonly IYahooDataService _yahooDataService;

        public InvestissementService(IDbContextFactory<InvestissementDbContext> dbContext,  IYahooDataService yahooDataService)
        {
            _dbFactory = dbContext;
            _yahooDataService = yahooDataService;
        }

        public async Task<IEnumerable<TransactionVM>> GetTransactions()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return await context.Transaction.Select(t => new TransactionVM
            {
                Date = t.Date.Value,
                Actif = t.Actif,
                Ticker =  t.Ticker,
                Prix = t.Prix,
                Quantite = t.Type == "Achat" ? t.Quantite : -t.Quantite,
            }).ToListAsync();
        }

        public async Task<IEnumerable<InvestissementParMois>> GetInvestissementParMois(decimal investissementMoyenMensuel)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var investissementParMois =  await CalculerInvestissementParMois();
            investissementParMois.ForEach(i => i.InvestissementMoyen = Math.Round(investissementMoyenMensuel,2));
            return investissementParMois;
        }

        public async Task<Dictionary<string,decimal>> GetPrixParActif()
        {
            var tickers = await GetTickers();
            return await _yahooDataService.GetPrixActuelAsync(tickers);
        }

        public async Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var transactions = await GetTransactions();

            return transactions.Sum(a => a.Quantite * prixParActif[a.Ticker]) ?? 0;
        }

        public async Task<decimal> CalculerValeurInvestissementTotal()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.Transaction
                .SumAsync(t => t.Type == "Achat" ? t.Total : -t.Total) ?? 0;
        }
        
        public async Task<decimal> CalculerInvestissementMoyenMensuel()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var investissementParMois = await CalculerInvestissementParMois();
            
            // on enlève le premier mois parce que j'avais quasi pas investit
            investissementParMois.RemoveAt(0);
            // on enlève le mois en cours pour que ça fausse pas trop la moyenne
            investissementParMois.RemoveAt(investissementParMois.Count - 1);
            
            return Math.Round(investissementParMois.Average(i => i.Investissement), 0);
        }

        public async Task<decimal> CalculerVariationParActif()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var investissementParMois = await CalculerInvestissementParMois();

            // on enlève le premier mois parce que j'avais quasi pas investit
            investissementParMois.RemoveAt(0);
            // on enlève le mois en cours pour que ça fausse pas trop la moyenne
            investissementParMois.RemoveAt(investissementParMois.Count - 1);

            return Math.Round(investissementParMois.Average(i => i.Investissement), 0);
        }

        public async Task<IEnumerable<InfoInvestParActif>> CalculerInfosInvestParActif(Dictionary<string,decimal> prixParActif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var rawData = await context.Transaction
                .GroupBy(t => new { t.Actif, t.Ticker })
                .Select(g => new
                {
                    g.Key.Actif,
                    g.Key.Ticker,
                    TotalQuantite = g.Sum(t => t.Type == "Achat" ? (decimal)t.Quantite : (decimal)-t.Quantite),
                    TotalInvesti = g.Sum(t => t.Type == "Achat" ? (decimal)(t.Quantite * t.Prix) : (decimal)(-t.Quantite * t.Prix))
                })
                .ToListAsync();

            return rawData.Where(t => t.TotalQuantite > 0).Select(t =>
            {
                var prixActuel = prixParActif[t.Ticker];
                var valeurDetenue = t.TotalQuantite * prixActuel;

                return new InfoInvestParActif
                {
                    Actif = t.Actif,
                    QuantiteDetenue = t.TotalQuantite,
                    PrixAchatMoyen = Math.Round((decimal)(t.TotalInvesti / t.TotalQuantite), 2),
                    PrixActuel = prixActuel,
                    ValeurDetenue = Math.Round((decimal)(t.TotalQuantite * prixParActif[t.Ticker]),2),
                    VariationValeur = Math.Round((decimal)(valeurDetenue - t.TotalInvesti), 2),
                    VariationPourcentage = Math.Round((decimal)((valeurDetenue - t.TotalInvesti) / t.TotalInvesti * 100), 2)

                };
            }).ToList();
        }

        public async Task AddTransactionsRange(IEnumerable<Transaction> transactions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var idsExistants = await context.Transaction.Select(t => t.Id).ToListAsync();
            var hashSetIds = new HashSet<string>(idsExistants);

            foreach (var transaction in transactions)
            {
                if (!hashSetIds.Contains(transaction.Id))
                {
                    await context.Transaction.AddAsync(transaction);
                    hashSetIds.Add(transaction.Id);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task AddFluxTradeRepublicRange(IEnumerable<FluxTradeRepublic> fluxBancaires)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var idsExistants = await context.FluxTradeRepublic.Select(t => t.Id).ToListAsync();
            var hashSetIds = new HashSet<string>(idsExistants);

            foreach (var flux in fluxBancaires)
            {
                if (!hashSetIds.Contains(flux.Id))
                {
                    await context.FluxTradeRepublic.AddAsync(flux);
                    hashSetIds.Add(flux.Id);
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task<IEnumerable<string>> GetTickers()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return context.Transaction.GroupBy(t => t.Actif).Select(d => d.First().Ticker).ToList();
        }

        private async Task<List<InvestissementParMois>> CalculerInvestissementParMois()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var rawData = await context.Transaction
                .GroupBy(t => new { t.Date.Value.Year, t.Date.Value.Month })
                .Select(d => new
                {
                    Annee = d.Key.Year,
                    Mois = d.Key.Month,
                    TotalInvesti = Math.Round(d.Sum(t => t.Type == "Achat" ? t.Total : -t.Total) ?? 0, 2)
                })
                .ToListAsync();

            return rawData
                .Select(d => new InvestissementParMois
                {
                    Date = new DateTime(d.Annee, d.Mois, 1),
                    Investissement = d.TotalInvesti
                })
                .OrderByDescending(d => d.Date)
                .ToList();
        }
    }
}
