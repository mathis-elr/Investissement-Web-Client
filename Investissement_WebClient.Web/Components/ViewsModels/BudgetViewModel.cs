using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.API.PowensApi;
using Investissement_WebClient.Application.Services.FluxBancaires;
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

        try
        {
            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;

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

    public void LoadFluxUnMois(StatutParMoisDto statutMoisDto)
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

    private async Task LoadCategories()
    {
        Categories = await _fluxBancaireService.GetCategorieFlux();
    }

    private async Task LoadDateLimiteValiditeSyncBanque()
    {
        DateExpirationSync = await _fluxBancaireService.GetDateLimiteValiditeSyncBanque(IdUser);
        ConnexionBanqueRequise = !DateExpirationSync.HasValue;
    }

    public void EditerMoisComplete()
    {
        StatutMoisActif!.Statut = Statut.edition;
        NotifyStateChanged();
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
            .Select(g => new ValeurParCategorieBarChartVM
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
            var unMois = new StatutParMoisDto { Date = dateCourante };

            bool estMoisCourant =
                dateCourante.Year == moisCourant.Year &&
                dateCourante.Month == moisCourant.Month;

            bool estMoisPrecedent =
                dateCourante.Year == moisPrecedent.Year &&
                dateCourante.Month == moisPrecedent.Month;

            bool estIndisponible =
                estMoisCourant ||
                (aujourdHui.Day < 5 && estMoisPrecedent);

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
}