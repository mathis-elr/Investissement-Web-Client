using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.FluxBancaires;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Web.GestionSession;

namespace Investissement_WebClient.Web.Components.ViewsModels;

public class BudgetViewModel(SessionService sessionService, 
                             IFluxBancaireService fluxBancaireService,
                             IPowensApiService powensApiService)
{
    private readonly SessionService _sessionService = sessionService;
    private readonly IFluxBancaireService _fluxBancaireService = fluxBancaireService;
    private readonly IPowensApiService _powensApiService = powensApiService;

    // USER CONNECTE
    public int IdUser { get; set; }
    public string PrenomUser { get; set; } = string.Empty;

    //MAJ VUE
    public event Action OnChange = null!;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public List<FluxBancaireVM> Flux { get; set; } = [];

    // RECAPITULATIF GLOBAL
    public IEnumerable<BudgetsParCategorieVM> BudgetLineCharts { get; set; } = [];

    // HISTORIQUE MENSUEL
    public DateTime DateDebut { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2);
    public List<StatutParMoisDto> StatutsParMois { get; set; } = [];
    public StatutParMoisDto? StatutMoisActif { get; set; } = null;
    public DateTime? DateActive { get; set; } = null;
    public string? DateActiveString => DateActive?.ToString("MMMM yyyy");

    // ENREGISTREMENT MENSUEL
    public DateTime? DateExpirationSync { get; set; } = null;
    public bool ConnexionBanqueRequise { get; set; } = false;
    public DateTime DateEditMensuel { get; set; } = DateTime.Today;
    public List<FluxBancaireVM> FluxMensuel { get; set; } = [];
    public List<FluxBancaireVM> CreditsFluxMensuel => FluxMensuel.Where(f => f.Valeur >= 0).ToList();
    public List<FluxBancaireVM> DebitsFluxMensuel => FluxMensuel.Where(f => f.Valeur < 0).ToList();
    public IEnumerable<CategorieFluxDto> Categories { get; set; } = [];
    public List<ValeurParCategorieBarChartVM> StatsGraphique { get; set; } = [];
    public bool ActionEnCours { get; set; } = false;

    // GESTION D'ERREUR
    public string MessageErreur { get; set; } = string.Empty;
    public bool HasErreur { get; set; } = false;

    public async Task StartLoadData()
    {
        ActionEnCours = true;
        await _sessionService.Initialiser();
        IdUser = _sessionService.Id;

        await LoadDateLimiteValiditeSyncBanque();

        await LoadFlux();
        await LoadBudgetParCategorie();

        DateDebut = Flux.Count != 0 ? Flux.Min(f => f.Date) : DateDebut;
        DeterminerStatutMois();

        ActionEnCours = false;
    }

    public async Task MajVue()
    {
        await LoadFlux();
        await LoadFluxUnMois(StatutMoisActif!);
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

    public async Task LoadFluxUnMois(StatutParMoisDto statutMoisDto)
    {
        StatutMoisActif = statutMoisDto;
        var date = statutMoisDto.Date;   

        DateActive = date;

        if (!Categories.Any())
            Categories = await _fluxBancaireService.GetCategorieFlux();

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

        await _powensApiService.GetFlux(dateDebut, dateFin, IdUser);

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

    private async Task LoadDateLimiteValiditeSyncBanque()
    {
        DateExpirationSync = await _fluxBancaireService.GetDateLimiteValiditeSyncBanque(IdUser);
        ConnexionBanqueRequise = !DateExpirationSync.HasValue;
        Console.WriteLine(ConnexionBanqueRequise);
    }

    public void EditerMoisComplete()
    {
        StatutMoisActif!.Statut = Statut.a_completer;
        NotifyStateChanged();
    }

    private async Task LoadBudgetParCategorie()
    {
        BudgetLineCharts = await _fluxBancaireService.CalculerBudgetCategorieParMois(IdUser);
    }

    private async Task RefreshData()
    {
        await LoadFlux();
        await LoadFluxUnMois(StatutMoisActif!);
        CalculerStatsGraphique();
    }

    private void CalculerStatsGraphique()
    {
        StatsGraphique = FluxMensuel
            .GroupBy(f => f.IdCategorie)
            .Select(g => new ValeurParCategorieBarChartVM
            {
                Categorie = Categories.FirstOrDefault(c => c.Id == g.Key)?.Libelle ?? "Inconnu",
                Valeur = g.Sum(f => f.Valeur),
            })
            .OrderByDescending(x => x.Valeur)
            .ToList();
    }

    private void DeterminerStatutMois()
    {
        StatutsParMois.Clear();
        var dateCourante =  DateTime.Now;

        while (dateCourante.Year >= DateDebut.Year && dateCourante.Month >= DateDebut.Month)
        {
            var DateLocale = dateCourante;
            var unMois = new StatutParMoisDto { Date = DateLocale };
            bool estIndisponible = (DateLocale.Year == DateTime.Now.Year && DateLocale.Month == DateTime.Now.Month);
            var fluxDuMois = Flux.Where(f => f.Date.Year == DateLocale.Year && f.Date.Month == DateLocale.Month).ToList();
            var fluxExiste = fluxDuMois.Count != 0;
            var allCategoriesCompletes = fluxDuMois.All(f => f.IdCategorie != 0);

            if (estIndisponible)
                unMois.Statut = Statut.indisponible;
            else if (fluxExiste && allCategoriesCompletes)
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
}