using System.Runtime.InteropServices.JavaScript;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.Graphiques;
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
            return await context.Transactions.Select(t => new TransactionVM
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
        
        private async Task<IEnumerable<string>> GetTickers()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return context.Transactions.GroupBy(t => t.Actif).Select(d => d.First().Ticker).ToList();
        }
        
        public async Task<decimal> CalculerValeurCourante()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
        
            var tickers = await GetTickers();
        
            var prixParActif = await _yahooDataService.GetPrixActuelAsync(tickers);

            var transactions = await GetTransactions();

            return transactions.Sum(a => a.Quantite * prixParActif[a.Ticker]) ?? 0;
        }

        public async Task<decimal> CalculerValeurInvestissementTotal()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.Transactions
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
            
            return investissementParMois.Average(i => i.Investissement);
        }

        private async Task<List<InvestissementParMois>> CalculerInvestissementParMois()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            
            var rawData = await context.Transactions
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

        public async Task AddTransactionsRange(IEnumerable<Transaction> transactions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            foreach (var transaction in transactions)
            {
                if (!await context.Transactions.AnyAsync(t => t.Id != transaction.Id))
                    await context.Transactions.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }

        public async Task AddFluxBancairesRange(IEnumerable<FluxBancaire> fluxBancaires)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            foreach (var transaction in fluxBancaires)
            {
                if (!await context.FluxBancaires.AnyAsync(t => t.Id != transaction.Id))
                    await context.FluxBancaires.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }
    }
}
