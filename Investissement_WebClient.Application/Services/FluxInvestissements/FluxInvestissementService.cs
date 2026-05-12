using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.YahooFinanceApi;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.FluxInvestissements
{
    public class FluxInvestissementService : IFluxInvestissementService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
        private readonly IYahooFinanceApiService _yahooDataService;

        private Dictionary<string, string> MappingsActifs = new Dictionary<string, string>
        {
            { "PEA MSCI Emerging Markets ESG EUR (Acc)", "ETF Emerging Markets" },
            { "Pea Monde MSCI World EUR (Acc)", "ETF MSCI World" }
        };

        public FluxInvestissementService(IDbContextFactory<InvestissementDbContext> dbContext,  IYahooFinanceApiService yahooDataService)
        {
            _dbFactory = dbContext;
            _yahooDataService = yahooDataService;
        }

        public async Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement.Select(t => new FluxInvestissementDto
            {
                Date = t.Date.Value,
                Actif = t.Actif,
                Ticker =  t.Ticker,
                Prix = t.Prix,
                Quantite = t.Type == "Achat" ? t.Quantite : -t.Quantite,
            }).ToListAsync();
        }

        public async Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(decimal investissementMoyenMensuel)
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

            var transactions = await GetFluxInvestissement();

            return transactions.Sum(a => a.Quantite * prixParActif[a.Ticker]) ?? 0;
        }

        public async Task<decimal> CalculerValeurInvestissementTotal()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .SumAsync(t => t.Type == "Achat" ? t.Total : -t.Total) ?? 0;
        }
        
        public async Task<decimal> CalculerInvestissementMoyenMensuel()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var investissementParMois = await CalculerInvestissementParMois();

            if (investissementParMois.Count == 0) return 0;

            // on enlève le premier mois parce que j'avais quasi pas investit
            investissementParMois.RemoveAt(0);
            // on enlève le mois en cours pour que ça fausse pas trop la moyenne
            investissementParMois.RemoveAt(investissementParMois.Count - 1);
            
            return Math.Round(investissementParMois.Average(i => i.Investissement), 0);
        }

        public async Task<IEnumerable<InfoValeurParActifDto>> CalculerInfosInvestParActif(Dictionary<string,decimal> prixParActif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var rawData = await context.FluxInvestissement
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

                return new InfoValeurParActifDto
                {
                    Actif = t.Actif,
                    ValeurDetenue = Math.Round(valeurDetenue, 2),
                    VariationValeur = Math.Round(valeurDetenue - t.TotalInvesti, 2),
                    VariationPourcentage = Math.Round((valeurDetenue - t.TotalInvesti) / t.TotalInvesti * 100, 2)

                };
            }).ToList();
        }

        public async Task AddFluxInvestissementRange(IEnumerable<FluxInvestissement> transactions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var idsExistants = await context.FluxInvestissement.Select(t => t.Id).ToListAsync();
            var hashSetIds = new HashSet<string>(idsExistants);

            foreach (var transaction in transactions)
            {
                if (!hashSetIds.Contains(transaction.Id))
                {
                    if(MappingsActifs.ContainsKey(transaction.Actif))
                        transaction.Actif = MappingsActifs[transaction.Actif];    

                    await context.FluxInvestissement.AddAsync(transaction);
                    hashSetIds.Add(transaction.Id);
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task<IEnumerable<string>> GetTickers()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            return context.FluxInvestissement.GroupBy(t => t.Actif).Select(d => d.First().Ticker).ToList();
        }

        private async Task<List<InvestissementParMoisVM>> CalculerInvestissementParMois()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var rawData = await context.FluxInvestissement
                .GroupBy(t => new { t.Date.Value.Year, t.Date.Value.Month })
                .Select(d => new
                {
                    Annee = d.Key.Year,
                    Mois = d.Key.Month,
                    TotalInvesti = Math.Round(d.Sum(t => t.Type == "Achat" ? t.Total : -t.Total) ?? 0, 2)
                })
                .ToListAsync();

            return rawData
                .Select(d => new InvestissementParMoisVM
                {
                    Date = new DateTime(d.Annee, d.Mois, 1),
                    Investissement = d.TotalInvesti
                })
                .OrderByDescending(d => d.Date)
                .ToList();
        }
    }
}
