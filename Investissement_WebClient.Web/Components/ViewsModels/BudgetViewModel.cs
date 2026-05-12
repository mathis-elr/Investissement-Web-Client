using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.Services.FluxBancaires;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Budgets;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Web.Components.ViewsModels;

public class BudgetViewModel(IFluxBancaireService fluxBancaireService, IPowensApiService powensApiService)
{
    private readonly IFluxBancaireService _fluxBancaireService = fluxBancaireService;
    private readonly IPowensApiService _powensApiService = powensApiService;

    //MAJ VUE
    public event Action OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();

    public List<FluxBancaireVM> FluxCreditCoop { get; set; } = [];

    // RECAPITULATIF GLOBAL
    public IEnumerable<BudgetsParCategorieVM> BudgetLineCharts { get; set; } = [];

    // HISTORIQUE MENSUEL
    public DateTime DateDebut { get; set; } = DateTime.Now.AddMonths(-3);
    public List<StatutParMoisDto> StatutsParMois { get; set; } = [];
    public StatutParMoisDto? StatutMoisActif { get; set; } = null;
    public DateTime? DateActive { get; set; } = null;
    public string? DateActiveString => DateActive?.ToString("MMMM yyyy");

    // ENREGISTREMENT MENSUEL
    public bool ConnexionBanqueRequise { get; set; }
    public DateTime DateEditMensuel { get; set; } = DateTime.Now;
    public List<FluxBancaireVM> FluxMensuel { get; set; } = [];
    public List<FluxBancaireVM> CreditsFluxMensuel => FluxMensuel.Where(f => f.Valeur >= 0).ToList();
    public List<FluxBancaireVM> DebitsFluxMensuel => FluxMensuel.Where(f => f.Valeur < 0).ToList();
    public IEnumerable<CategorieFluxDto> Categories { get; set; } = [];
    public List<ValeurParCategorieBarChartVM> StatsGraphique { get; set; } = [];

    // GESTION D'ERREUR
    public string MessageErreur { get; set; } = string.Empty;
    public bool HasErreur { get; set; } = false;

    public async Task StartLoadData()
    {
        await LoadConnexionBanqueNecessaire();
        await LoadFluxCreditCoop();
        await LoadBudgetParCategorie();

        DateDebut = FluxCreditCoop.Count != 0 ? FluxCreditCoop.Min(f => f.Date) : DateDebut;
        DeterminerStatutMois();
    }

    public async Task LoadConnexionBanqueNecessaire()
    {
        ConnexionBanqueRequise = await _powensApiService.ConnexionRequise();
    }

    public void SetRecapGlobalMode()
    {
        DateActive = null;
        StatutMoisActif = null;

        NotifyStateChanged();
    }

    public async Task LoadFluxUnMois(StatutParMoisDto statutMoisDto)
    {
        StatutMoisActif = statutMoisDto;
        var date = statutMoisDto.Date;   

        DateActive = date;

        if(!Categories.Any())
            Categories = await _fluxBancaireService.GetCategorieFlux();

        DateEditMensuel = date;
        FluxMensuel = FluxCreditCoop
            .Where(f => f.Date.Month == date.Month && f.Date.Year == date.Year)
            .OrderByDescending(f => f.Date)
            .ToList();

        if (statutMoisDto.Statut == Statut.complete)
            CalculerStatsGraphique();

        NotifyStateChanged();
    }

    public async Task LoadFluxCreditCoop()
    {
        FluxCreditCoop = await _fluxBancaireService.GetFluxBancaire();
    }

    public async Task GetFluxMensuel(DateTime date)
    {
        if (ConnexionBanqueRequise)
        {
            HasErreur = true;
            MessageErreur = "Cette action nécéssite la synchronisation avec vôtre banque";
            return;
        }

        var dateDebut = new DateTime(date.Year, date.Month, 1);
        var dernierJourDuMois = DateTime.DaysInMonth(date.Year, date.Month);
        var dateFin = new DateTime(date.Year, date.Month, dernierJourDuMois);

        await _powensApiService.GetFlux(dateDebut, dateFin);

        await RefreshData();
        NotifyStateChanged();
    }

    public async Task UpdateFluxMensuel()
    {
        if (FluxMensuel == null)
            throw new Exception("Aucune données mensuel");

        await _fluxBancaireService.UpdateFluxCreditCoopMensuel(FluxMensuel);

        await RefreshData();
        DeterminerStatutMois();
        NotifyStateChanged();
    }

    public void EditerMoisComplete()
    {
        StatutMoisActif.Statut = Statut.en_cours;
        NotifyStateChanged();
    }

    private async Task LoadBudgetParCategorie()
    {
        BudgetLineCharts = await _fluxBancaireService.CalculerBudgetCategorieParMois();
    }

    private async Task RefreshData()
    {
        await LoadFluxCreditCoop();
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
            var fluxDuMois = FluxCreditCoop.Where(f => f.Date.Year == DateLocale.Year && f.Date.Month == DateLocale.Month).ToList();
            var fluxExiste = fluxDuMois.Count != 0;
            var allCategoriesCompletes = fluxDuMois.All(f => f.IdCategorie != 0);

            if (estIndisponible)
                unMois.Statut = Statut.indisponible;
            else if (fluxExiste && allCategoriesCompletes)
                unMois.Statut = Statut.complete;
            else if (fluxExiste)
                unMois.Statut = Statut.en_cours;
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