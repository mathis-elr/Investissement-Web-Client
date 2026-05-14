using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.Actifs;
using Investissement_WebClient.Application.Services.YahooFinanceApi;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.FluxInvestissements
{
    public class FluxInvestissementService : IFluxInvestissementService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;
        private readonly IYahooFinanceApiService _yahooFinanceApiService;
        private readonly IActifService _actifService;

        public FluxInvestissementService(IDbContextFactory<InvestissementDbContext> dbContext,  
                                         IYahooFinanceApiService yahooFinanceApiService,
                                         IActifService actifService)
        {
            _dbFactory = dbContext;
            _yahooFinanceApiService = yahooFinanceApiService;
            _actifService = actifService;
        }

        public async Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Include(f => f.Actif)
                .Select(t => new FluxInvestissementDto
                {
                    Date = t.Date,
                    Actif = t.Actif.Libelle,
                    Ticker = t.Actif.Ticker,
                    Prix = t.Prix,
                    Quantite = t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite,
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
            var tickers = await _actifService.GetTickers();
            return await _yahooFinanceApiService.GetPrixActuelAsync(tickers);
        }

        public async Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var data = await context.FluxInvestissement
                .Include(f => f.Actif)
                .GroupBy(t => new { t.Actif.Libelle, t.Actif.Ticker })
                .Select(groupe => new
                {
                    groupe.Key.Libelle,
                    groupe.Key.Ticker,
                    QuantiteTotale = groupe.Sum(t => t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite)
                })
                .ToListAsync();

            return data.Where(t => t.QuantiteTotale != 0).Select(t => new ValeurTotaleParActifVM
            {
                Actif = t.Libelle,
                Valeur = Math.Round((decimal)(t.QuantiteTotale * (prixParActif.TryGetValue(t.Ticker, out decimal value) ? value : 0)), 2)
            }).ToList();
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
                .SumAsync(t => t.Type == TypeFlux.Achat ? t.Total : -t.Total);
        }
        
        public async Task<decimal> CalculerInvestissementMoyenMensuel()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var investissementParMois = await CalculerInvestissementParMois();

            if (investissementParMois.Count == 0)
                return 0;

            // on retire le mois en cours
            investissementParMois.RemoveAt(investissementParMois.Count - 1);

            return Math.Round(investissementParMois.Average(i => i.Investissement), 0);
        }

        public async Task<IEnumerable<InfoValeurParActifDto>> CalculerInfosInvestParActif(Dictionary<string,decimal> prixParActif)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var rawData = await context.FluxInvestissement
                .GroupBy(t => new { t.Actif.Libelle, t.Actif.Ticker })
                .Select(g => new
                {
                    g.Key.Libelle,
                    g.Key.Ticker,
                    TotalQuantite = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)t.Quantite : (decimal)-t.Quantite),
                    TotalInvesti = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)(t.Quantite * t.Prix) : (decimal)(-t.Quantite * t.Prix))
                })
                .ToListAsync();

            return rawData.Where(t => t.TotalQuantite > 0).Select(t =>
            {
                var prixActuel = prixParActif[t.Ticker];
                var valeurDetenue = t.TotalQuantite * prixActuel;

                return new InfoValeurParActifDto
                {
                    Actif = t.Libelle,
                    ValeurDetenue = Math.Round(valeurDetenue, 2),
                    VariationValeur = Math.Round(valeurDetenue - t.TotalInvesti, 2),
                    VariationPourcentage = Math.Round((valeurDetenue - t.TotalInvesti) / t.TotalInvesti * 100, 2)

                };
            }).ToList();
        }

        public async Task MapperTransactions(List<TradeRepublicUnFluxApiResponse> transactions)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var actifsLoacaux = await _actifService.GetAll();
            var tickersExistants = actifsLoacaux.Select(a => a.Ticker);

            var transactionsValides = transactions
        .       Where(t => t.Date.HasValue 
                        && t.Prix.HasValue
                        && t.Quantite.HasValue
                        && t.Actif != null
                        && t.Ticker != null
                        && t.ISIN != null);

            foreach (var transaction in transactionsValides)
            {
                var nvFlux = new FluxInvestissement
                {
                    Date = transaction.Date!.Value.DateTime,
                    Type = transaction.Type == "Achat" ? TypeFlux.Achat : TypeFlux.Vente,
                    Prix = transaction.Prix!.Value,
                    Quantite = transaction.Quantite!.Value,
                    Frais = transaction.Frais,
                    Total = transaction.Total ?? (transaction.Prix!.Value * transaction.Quantite!.Value)
                };

                var IdActif = actifsLoacaux.FirstOrDefault(a => a.Ticker == transaction.Ticker)?.Id;
                if(IdActif == null)
                {
                    var nvActif = new Actif
                    {
                        Libelle = transaction.Actif!,
                        ISIN = transaction.ISIN!,
                        Ticker = transaction.Ticker!
                    };
                    nvFlux.ActifId = await _actifService.AddActif(nvActif);
                    actifsLoacaux.Add(nvActif);
                }
                else
                {
                    nvFlux.ActifId = IdActif.Value;
                }
                context.FluxInvestissement.Add(nvFlux);
            }
            await context.SaveChangesAsync();
        }

        public async Task AddFluxInvestissementRange(IEnumerable<FluxInvestissement> flux)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var idsExistants = await context.FluxInvestissement.Select(t => t.Id).ToListAsync();
            var hashSetIds = new HashSet<string>(idsExistants);

            var nouveauxFlux = flux.Where(f => !hashSetIds.Contains(f.Id)).ToList();

            if (nouveauxFlux.Count != 0)
            {
                await context.FluxInvestissement.AddRangeAsync(nouveauxFlux);
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
        }

        private async Task<List<InvestissementParMoisVM>> CalculerInvestissementParMois()
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var rawData = await context.FluxInvestissement
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(d => new
                {
                    Annee = d.Key.Year,
                    Mois = d.Key.Month,
                    TotalInvesti = Math.Round(d.Sum(t => t.Type == TypeFlux.Achat ? t.Total : -t.Total), 2)
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
