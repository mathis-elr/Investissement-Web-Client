using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Infrastructure.APIs.Powens;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Investissement_WebClient.Web.Components.ViewsModels.Budget
{
    public class BudgetViewModel(IUtilisateurPowensRepository utilisateurPowensRepository,
                                 IFluxBancaireService fluxBancaireService,
                                 IOptions<PowensApiOptions> options,
                                 IPowensApiService powensApiService,
                                 SessionService sessionService)
    {
        private readonly IUtilisateurPowensRepository _utilisateurPowensRepository = utilisateurPowensRepository;
        private readonly IFluxBancaireService _fluxBancaireService = fluxBancaireService;
        private readonly PowensApiOptions _powensApiOptions = options.Value;
        private readonly IPowensApiService _powensApiService = powensApiService;
        private readonly SessionService _sessionService = sessionService;

        // CONNEXION BANQUE
        public string UrlConnexionPowens { get; set; } = string.Empty;

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        //MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public List<FluxBancaireDto> Flux { get; set; } = [];

        // RECAPITULATIF GLOBAL
        public IEnumerable<PeriodeBudget> PeriodeBudgetPossibles => Enum.GetValues<PeriodeBudget>();
        public PeriodeBudget PeriodeBudgetSelectionnee { get; set; } = PeriodeBudget.Tout;

        public IEnumerable<BudgetsParCategorieDto> BudgetLineCharts { get; set; } = [];
        public List<string> CouleursGraphique = new List<string> { "#22c55e", "#3b82f6", "#ef4444", "#eab308" };

        public decimal RevenusMoyens => BudgetLineCharts
            .FirstOrDefault(b => b.Categorie.Equals("Revenus", StringComparison.OrdinalIgnoreCase))?
            .BudgetCategorieParMois
            .Average(b => b.Budget) ?? 0;

        public decimal DepensesMoyennnes => BudgetLineCharts
            .Where(b => b.Categorie.Equals("Vie quotidienne", StringComparison.OrdinalIgnoreCase)
                     || b.Categorie.Equals("Loisirs/Plaisirs", StringComparison.OrdinalIgnoreCase)) 
            .SelectMany(b => b.BudgetCategorieParMois)
            .GroupBy(m => m.Date)
            .Select(g => g.Sum(m => m.Budget))
            .DefaultIfEmpty(0)
            .Average();

        public decimal TauxEpargne => BudgetLineCharts
            .SelectMany(b => b.BudgetCategorieParMois, (b, m) => new { b.Categorie, m.Budget })
            .GroupBy(_ => 1)
            .Select(g =>
            {
                var revenus = g.Where(x => x.Categorie.Equals("Revenus", StringComparison.OrdinalIgnoreCase))
                               .Sum(x => x.Budget);
                var epargne = g.Where(x => x.Categorie.Equals("Patrimoine", StringComparison.OrdinalIgnoreCase))
                               .Sum(x => x.Budget);
                return revenus == 0 ? 0 : Math.Abs(epargne) / revenus;
            })
            .FirstOrDefault();

        public decimal SoldeMoyenFinMois => BudgetLineCharts
            .SelectMany(b => b.BudgetCategorieParMois)
            .GroupBy(m => m.Date)
            .Select(g => g.Sum(m => m.Budget))
            .DefaultIfEmpty(0)
            .Average();


        // HISTORIQUE MENSUEL
        public DateTime DateDebut { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2);
        public List<StatutParMoisVM> StatutsParMois { get; set; } = [];
        public StatutParMoisVM? StatutMoisActif { get; set; } = null;
        public DateTime? DateActive { get; set; } = null;
        public string? DateActiveString => DateActive?.ToString("MMMM yyyy");

        // ENREGISTREMENT MENSUEL
        public DateTime? DateExpirationSync { get; set; } = null;
        public bool ConnexionBanqueRequise { get; set; } = false;
        public DateTime DateEditMensuel { get; set; } = DateTime.Today;
        public List<FluxBancaireDto> FluxMensuel { get; set; } = [];
        public List<FluxBancaireDto> CreditsFluxMensuel => FluxMensuel.Where(f => f.Valeur >= 0).ToList();
        public List<FluxBancaireDto> DebitsFluxMensuel => FluxMensuel.Where(f => f.Valeur < 0).ToList();
        public IEnumerable<CategorieFluxDto> Categories { get; set; } = [];
        public List<ValeurParCategorieBarChartDto> StatsGraphique { get; set; } = [];
        public bool ActionEnCours { get; set; } = false;

        // GESTION D'ERREUR
        public string MessageErreur { get; set; } = string.Empty;
        public bool HasErreur { get; set; } = false;


        public async Task FinaliserAjoutBanque(int connectionBanqueId)
        {
            await InitialiserSession();

            try
            {
                await _powensApiService.SaveBanque(connectionBanqueId, IdUser);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }

        public async Task StartLoadData()
        {
            ActionEnCours = true;

            try
            {
                await InitialiserSession();

                await LoadDateLimiteValiditeSyncBanque();

                await Task.WhenAll(
                    LoadFlux(),
                    LoadBudgetParCategorie(),
                    LoadCategories()
                );

                DateDebut = Flux.Count != 0 ? Flux.Min(f => f.Date) : DateDebut;
                DeterminerStatutMois();
            }
            finally
            {
                ActionEnCours = false;
            }
        }

        public async Task MajVue()
        {
            await LoadFlux();
            LoadFluxUnMois(StatutMoisActif!);
            DeterminerStatutMois();
            ActionEnCours = false;

            NotifyStateChanged();
        }

        public void SetRecapGlobalMode()
        {
            FluxMensuel = [];
            DateActive = null;
            StatutMoisActif = null;

            NotifyStateChanged();
        }

        public void LoadFluxUnMois(StatutParMoisVM statutMoisDto)
        {
            StatutMoisActif = statutMoisDto;
            var date = statutMoisDto.Date;

            DateActive = date;

            DateEditMensuel = date;
            FluxMensuel = Flux
                .Where(f => f.Date.Month == date.Month && f.Date.Year == date.Year)
                .OrderByDescending(f => f.Date)
                .ToList();

            if (statutMoisDto.Statut == Statut.complete)
                CalculerStatsGraphique();

            NotifyStateChanged();
        }

        public async Task LoadFlux()
        {
            Flux = await _fluxBancaireService.GetFluxBancaire(IdUser);
        }

        public async Task GetFluxMensuel()
        {
            ActionEnCours = true;

            if (ConnexionBanqueRequise)
            {
                HasErreur = true;
                MessageErreur = "Cette action nécéssite la synchronisation avec vôtre banque";
                return;
            }

            var dateDebut = new DateTime(DateActive!.Value.Year, DateActive.Value.Month, 1);
            var dernierJourDuMois = DateTime.DaysInMonth(DateActive.Value.Year, DateActive.Value.Month);
            var dateFin = new DateTime(DateActive.Value.Year, DateActive.Value.Month, dernierJourDuMois);

            //await GetFlux(dateDebut, dateFin);

            await RefreshData();
            NotifyStateChanged();
        }

        public async Task UpdateFluxMensuel()
        {
            ActionEnCours = true;

            if (FluxMensuel == null)
                throw new Exception("Aucune données mensuel");

            await _fluxBancaireService.UpdateFluxMensuel(FluxMensuel, IdUser);

            await RefreshData();
            DeterminerStatutMois();

            ActionEnCours = false;

            NotifyStateChanged();
        }

        public async Task GetFlux(DateTime dateDebut, DateTime dateFin)
        {
            //await _powensApiService.GetFlux(dateDebut, dateFin, IdUser);
        }

        public void EditerMoisComplete()
        {
            StatutMoisActif!.Statut = Statut.edition;
            NotifyStateChanged();
        }

        public string GetLibellePeriodeBudget(PeriodeBudget periode)
        {
            return periode switch
            {
                PeriodeBudget.TroisMois => "3M",
                PeriodeBudget.SixMois => "6M",
                PeriodeBudget.Ans => "1A",
                PeriodeBudget.Tout => "Tout",
                _ => string.Empty
            };
        }

        public async Task ChangerPeriodeBudgetSelectionnee(PeriodeBudget periode)
        {
            if (PeriodeBudgetSelectionnee == periode)
                return;

            PeriodeBudgetSelectionnee = periode;

            await RafraichirGraphique();
        }

        public async Task InitialiserUrlConnexionPowens()
        {
            UrlConnexionPowens = await GetUrlConnexionPowens();
        }

        private async Task InitialiserSession()
        {
            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;
        }

        private async Task RafraichirGraphique()
        {
            //ChargementGraphique = true;

            //try
            //{
            //    await LoadInvestissementsParMois();

            //    NotifyStateChanged();
            //}
            //finally
            //{
            //    ChargementGraphique = false;
            //}
        }

        private async Task LoadCategories()
        {
            Categories = await _fluxBancaireService.GetCategorieFlux();
        }

        private async Task LoadDateLimiteValiditeSyncBanque()
        {
            DateExpirationSync = await _fluxBancaireService.GetDateLimiteValiditeSyncBanque(IdUser);
            ConnexionBanqueRequise = !DateExpirationSync.HasValue;
        }

        private async Task LoadBudgetParCategorie()
        {
            BudgetLineCharts = await _fluxBancaireService.CalculerBudgetCategorieParMois(IdUser);
        }

        private async Task RefreshData()
        {
            await LoadFlux();
            LoadFluxUnMois(StatutMoisActif!);
            CalculerStatsGraphique();
        }

        private void CalculerStatsGraphique()
        {
            StatsGraphique = FluxMensuel
                .GroupBy(f => f.IdCategorie)
                .Select(g => new ValeurParCategorieBarChartDto
                {
                    Categorie = Categories.FirstOrDefault(c => c.Id == g.Key)?.Libelle ?? "Inconnu",
                    Valeur = g.Sum(f => f.Valeur),
                })
                .Where(g => g.Valeur != 0)
                .OrderByDescending(x => x.Valeur)
                .ToList();
        }

        private void DeterminerStatutMois()
        {
            StatutsParMois.Clear();

            var aujourdHui = DateTime.Now;
            var moisCourant = new DateTime(aujourdHui.Year, aujourdHui.Month, 1);
            var moisPrecedent = moisCourant.AddMonths(-1);

            var dateCourante = moisCourant;

            while (dateCourante >= new DateTime(DateDebut.Year, DateDebut.Month, 1))
            {
                var unMois = new StatutParMoisVM { Date = dateCourante };

                bool estMoisCourant =
                    dateCourante.Year == moisCourant.Year &&
                    dateCourante.Month == moisCourant.Month;

                bool estMoisPrecedent =
                    dateCourante.Year == moisPrecedent.Year &&
                    dateCourante.Month == moisPrecedent.Month;

                bool estIndisponible =
                    estMoisCourant ||
                    aujourdHui.Day < 5 && estMoisPrecedent;

                var fluxDuMois = Flux
                    .Where(f => f.Date.Year == dateCourante.Year &&
                                f.Date.Month == dateCourante.Month)
                    .ToList();

                bool fluxExiste = fluxDuMois.Count != 0;
                bool toutesCategoriesCompletes = fluxDuMois.All(f => f.IdCategorie != 0);

                if (estIndisponible)
                    unMois.Statut = Statut.indisponible;
                else if (fluxExiste && toutesCategoriesCompletes)
                    unMois.Statut = Statut.complete;
                else if (fluxExiste)
                    unMois.Statut = Statut.a_completer;
                else
                    unMois.Statut = Statut.aucune_donnees;

                StatutsParMois.Add(unMois);

                dateCourante = dateCourante.AddMonths(-1);
            }

            if (DateActive.HasValue)
            {
                StatutMoisActif = StatutsParMois.FirstOrDefault(m =>
                    m.Date.Month == DateActive.Value.Month &&
                    m.Date.Year == DateActive.Value.Year);
            }
        }

        private async Task<string> GetUrlConnexionPowens()
        {
            await _powensApiService.VerifierUtilisateurPowensExists(IdUser);

            var code = await _powensApiService.GenerateCodeTemporaireByUserId(IdUser);

            var fullConnectUrl = new Uri(new Uri(_powensApiOptions.BaseUri), _powensApiOptions.ConnectEndPoint);

            var encodedRedirect =
                Uri.EscapeDataString(_powensApiOptions.RedirectUri);

            return $"{fullConnectUrl}?client_id={_powensApiOptions.ClientId}&redirect_uri={encodedRedirect}&code={code}";
        }
    }
}