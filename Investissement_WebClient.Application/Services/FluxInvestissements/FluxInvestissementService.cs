using Investissement_WebClient.Application.ViewsModels.Graphiques.Investissements;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Patrimoines;
using Investissement_WebClient.Application.Services.API.YahooFinanceApi;
using Investissement_WebClient.Application.ApiResponse.TradeRepublic;
using Investissement_WebClient.Application.Services.Actifs;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Investissement_WebClient.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Application.Services.FluxInvestissements
{
    public class FluxInvestissementService(IDbContextFactory<InvestissementDbContext> dbContext,
                                           IYahooFinanceApiService yahooFinanceApiService,
                                           IActifService actifService) : IFluxInvestissementService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory = dbContext;
        private readonly IYahooFinanceApiService _yahooFinanceApiService = yahooFinanceApiService;
        private readonly IActifService _actifService = actifService;

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

        public async Task<IEnumerable<InvestissementParMoisVM>> GetInvestissementParMois(int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();
            var investissementParMois =  await CalculerInvestissementParMois(userId);
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
                Valeur = Math.Round(t.QuantiteTotale * (prixParActif.TryGetValue(t.Ticker, out decimal value) ? value : 0), 2)
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
        
        public async Task<decimal> CalculerInvestissementMedianMensuel(int userId)
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

        public async Task<IEnumerable<ValeurActifInfosDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif, int userId)
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            var infosParActif = await context.FluxInvestissement
                .Include(f => f.Actif)
                .Where(f => f.UtilisateurId == userId)
                .GroupBy(t => new { t.Actif!.Libelle, t.Actif.Ticker })
                .Select(g => new
                {
                    g.Key.Libelle,
                    g.Key.Ticker,
                    TotalQuantite = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)t.Quantite : (decimal)-t.Quantite),
                    TotalValeurInvest = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)(t.Quantite * t.Prix) : (decimal)(-t.Quantite * t.Prix))
                })
                .ToListAsync();

            var actifsValides = infosParActif.Where(t => t.TotalQuantite > 0).ToList();

            var tasks = actifsValides.Select(async t =>
            {
                var prixActuel = prixParActif[t.Ticker];
                var valeurActuelle = t.TotalQuantite * prixActuel;

                var variationsParLapsTemps = new Dictionary<LapsTemps, VariationDataDto>();
                var prixParActifHistorique = await _yahooFinanceApiService.GetPrixHistorique(t.Ticker);

                foreach (LapsTemps periode in Enum.GetValues(typeof(LapsTemps)))
                {
                    if (periode == LapsTemps.All)
                    {
                        variationsParLapsTemps[LapsTemps.All] = new VariationDataDto
                        {
                            VariationValeur = valeurActuelle - t.TotalValeurInvest,
                            VariationPourcentage = Math.Round(((valeurActuelle - t.TotalValeurInvest) / t.TotalValeurInvest) * 100, 2)
                        };
                        continue;
                    }

                    variationsParLapsTemps[periode] = CalculVariationPrix(prixParActifHistorique[periode], prixActuel);
                }

                return new ValeurActifInfosDto
                {
                    Actif = t.Libelle,
                    ValeurInvestit = Math.Round(valeurActuelle, 2),
                    VariationsParLapsTemps = variationsParLapsTemps
                };
            }).ToList(); 

            return await Task.WhenAll(tasks);
        }

        private VariationDataDto CalculVariationPrix(decimal prixHistorique, decimal prixCourant)
        {
            return new VariationDataDto
            {
                VariationValeur = prixCourant - prixHistorique,
                VariationPourcentage = Math.Round(((prixCourant - prixHistorique) / prixHistorique) * 100, 2)
            };
        }

        //public async Task<IEnumerable<InfoParActifDto>> CalculerInfosInvestParActif(Dictionary<string, decimal> prixParActif, int userId)
        //{
        //    await using var context = await _dbFactory.CreateDbContextAsync();
        //    var rawData = await context.FluxInvestissement
        //        .Where(f => f.UtilisateurId == userId)
        //        .GroupBy(t => new { t.Actif!.Libelle, t.Actif.Ticker })
        //        .Select(g => new
        //        {
        //            g.Key.Libelle,
        //            g.Key.Ticker,
        //            TotalQuantite = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)t.Quantite : (decimal)-t.Quantite),
        //            TotalInvesti = g.Sum(t => t.Type == TypeFlux.Achat ? (decimal)(t.Quantite * t.Prix) : (decimal)(-t.Quantite * t.Prix))
        //        })
        //        .ToListAsync();

        //    return rawData.Where(t => t.TotalQuantite > 0).Select(t =>
        //    {
        //        var prixActuel = prixParActif[t.Ticker];
        //        var valeurDetenue = t.TotalQuantite * prixActuel;

        //        return new InfoParActifDto
        //        {
        //            Actif = t.Libelle,
        //            ValeurDetenue = Math.Round(valeurDetenue, 2),
        //            VariationValeur = Math.Round(valeurDetenue - t.TotalInvesti, 2),
        //            VariationPourcentage = Math.Round((valeurDetenue - t.TotalInvesti) / t.TotalInvesti * 100, 2)

        //        };
        //    }).ToList();
        //}

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
                    TotalInvestit = Math.Round(d.Sum(t => t.Type == TypeFlux.Achat ? t.Total : -t.Total), 2)
                })
                .ToListAsync();

            return rawData
                .Select(d => new InvestissementParMoisVM
                {
                    Date = new DateTime(d.Annee, d.Mois, 1),
                    Investissement = d.TotalInvestit
                })
                .OrderByDescending(d => d.Date)
                .ToList();
        }
    }
}
