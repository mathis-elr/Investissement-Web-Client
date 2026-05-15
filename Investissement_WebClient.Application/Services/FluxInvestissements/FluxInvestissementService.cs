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

        public async Task<IEnumerable<FluxInvestissementDto>> GetFluxInvestissement(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Include(f => f.Actif)
                .Where(f => f.UtilisateurId == userId)
                .Select(t => new FluxInvestissementDto
                {
                    Date = t.Date,
                    Actif = t.Actif!.Libelle,
                    Ticker = t.Actif.Ticker,
                    Prix = t.Prix,
                    Quantite = t.Type == TypeFlux.Achat ? t.Quantite : -t.Quantite,
                }).ToListAsync();
        }

        public async Task<string?> GetDernierFluxEnregistre(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var dernierFlux = await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .OrderByDescending(f => f.Date)
                .FirstOrDefaultAsync();
            return dernierFlux?.Id;
        }

        public async Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(decimal investissementMoyenMensuel, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var investissementParMois =  await CalculerInvestissementParMois(userId);
            investissementParMois.ForEach(i => i.InvestissementMoyen = Math.Round(investissementMoyenMensuel,2));
            return investissementParMois;
        }

        public async Task<Dictionary<string,decimal>> GetPrixParActif()
        {
            var tickers = await _actifService.GetTickers();
            if (!tickers.Any()) return [];
            return await _yahooFinanceApiService.GetPrixActuelAsync(tickers);
        }

        public async Task<IEnumerable<ValeurTotaleParActifVM>> GetValeurParActifInvestit(Dictionary<string, decimal> prixParActif, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var data = await context.FluxInvestissement
                .Include(f => f.Actif)
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new { t.Actif!.Libelle, t.Actif.Ticker })
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

        public async Task<decimal> CalculerValeurCourante(Dictionary<string, decimal> prixParActif, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var transactions = await GetFluxInvestissement(userId);

            return transactions.Sum(a => a.Quantite * prixParActif[a.Ticker!]);
        }

        public async Task<decimal> CalculerValeurInvestissementTotal(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            return await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .SumAsync(t => t.Type == TypeFlux.Achat ? t.Total : -t.Total);
        }
        
        public async Task<decimal> CalculerInvestissementMoyenMensuel(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var investissementParMois = await CalculerInvestissementParMois(userId);

            if (investissementParMois.Count == 0)
                return 0;

            var donneesCompletes = investissementParMois
                .Take(investissementParMois.Count - 1)
                .Select(i => i.Investissement)
                .ToList();

            var sorted = donneesCompletes.OrderBy(v => v).ToList();
            decimal mediane;
            int mid = sorted.Count / 2;
            mediane = sorted.Count % 2 != 0 ? sorted[mid] : (sorted[mid - 1] + sorted[mid]) / 2;

            return Math.Round(mediane, 0);
        }

        public async Task<IEnumerable<InfoValeurParActifDto>> CalculerInfosInvestParActif(Dictionary<string,decimal> prixParActif, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var rawData = await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new { t.Actif!.Libelle, t.Actif.Ticker })
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

        public async Task MapperTransactions(List<TradeRepublicUnFluxApiResponse> transactions, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var actifsLoacaux = await _actifService.GetAll();

            var transactionsValides = transactions
        .       Where(t => t.Id != null
                        && t.Date.HasValue 
                        && t.Prix.HasValue
                        && t.Quantite.HasValue
                        && t.Actif != null
                        && t.ISIN != null);

            foreach (var transaction in transactionsValides)
            {
                var nvFlux = new FluxInvestissement
                {
                    Id = transaction.Id,
                    Date = transaction.Date!.Value.DateTime,
                    Type = (TypeFlux)transaction.Type!,
                    Prix = transaction.Prix!.Value,
                    Quantite = transaction.Quantite!.Value,
                    Frais = transaction.Frais,
                    Total = transaction.Total ?? (transaction.Prix!.Value * transaction.Quantite!.Value),
                    UtilisateurId = userId
                };

                var IdActif = actifsLoacaux.FirstOrDefault(a => a.ISIN == transaction.ISIN)?.Id;
                if(IdActif == null)
                {
                    var ticker = await _yahooFinanceApiService.GetTickerByIsinAsync(transaction.ISIN!);
                    var nvActif = new Actif
                    {
                        Libelle = _actifService.NettoyerLibelle(transaction.Actif!),
                        ISIN = transaction.ISIN!,
                        Ticker = ticker!
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

        private async Task<List<InvestissementParMoisVM>> CalculerInvestissementParMois(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var rawData = await context.FluxInvestissement
                .Where(f => f.UtilisateurId == userId)
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
