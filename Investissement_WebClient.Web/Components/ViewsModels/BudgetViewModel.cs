using Investissement_WebClient.Application.Services.CreditCoop;
using Investissement_WebClient.Application.Services.Powens;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Web.Components.ViewsModels;

public class BudgetViewModel(IFluxCreditCoopService fluxCreditCoopService, IPowensDataService powensDataService)
{
    private readonly IFluxCreditCoopService _fluxCreditCoopService = fluxCreditCoopService;
    private readonly IPowensDataService _powensDataService = powensDataService;

    public List<FluxCreditCoopVM> FluxCreditCoop { get; set; } = [];

    // STATISTIQUES
    public bool ModeStatistique { get; set; } = true;

    // HISTORIQUE MENSUEL
    public DateTime DateDebut { get; set; } = DateTime.Now.AddMonths(-3);
    public List<StatutMoisVM> StatutsMois { get; set; } = [];

    public DateTime? DateSelectionnee { get; set; } = null;

    // ENREGISTREMENT MENSUEL
    public DateTime DateEditMensuel { get; set; } = DateTime.Now;
    public List<FluxCreditCoopVM> FluxMensuel { get; set; } = [];
    public List<FluxCreditCoopVM> CreditsFluxMensuel{ get; set; } = [];
    public List<FluxCreditCoopVM> DebitsFluxMensuel { get; set; } = [];
    public IEnumerable<CategorieFluxVM> Categories { get; set; } = [];


    public string MessageErreur { get; set; } = string.Empty;
    public bool HasErreur { get; set; } = false;


    public async Task StartLoadData()
    {
        await LoadFluxCreditCoop();

        LoadDateDebut();
        DeterminerStatutMois();
    }

    public void SetStatsMode()
    {
        ModeStatistique = true;
        DateSelectionnee = null;
    }

    public async Task LoadFluxUnMois(DateTime date)
    {
        DateSelectionnee = date;
        if(!Categories.Any())
            await LoadCategories();

        DateEditMensuel = date;
        FluxMensuel = FluxCreditCoop
            .Where(f => f.Date.Month == date.Month && f.Date.Year == date.Year)
            .OrderByDescending(f => f.Date)
            .ToList();

        LoadCreditFluxMensuel();
        LoadDebitsFluxMensuel();
        ModeStatistique = false;
    }

    private void LoadCreditFluxMensuel()
    {
        CreditsFluxMensuel = FluxMensuel.Where(f => f.Valeur >= 0).ToList();
    }

    private void LoadDebitsFluxMensuel()
    {
        DebitsFluxMensuel = FluxMensuel.Where(f => f.Valeur < 0).ToList();
    }

    private async Task LoadCategories()
    {
        Categories = await _fluxCreditCoopService.GetCategorieFlux();
    }

    private async Task LoadFluxCreditCoop()
    {
        FluxCreditCoop = await _fluxCreditCoopService.GetFlux();
    }

    private void LoadDateDebut()
    {
        DateDebut = FluxCreditCoop.Count != 0 ? FluxCreditCoop.Min(f => f.Date) : DateDebut;
    }

    private void DeterminerStatutMois()
    {
        StatutsMois.Clear();
        var dateCourante =  DateTime.Now;

        while (dateCourante.Year >= DateDebut.Year && dateCourante.Month >= DateDebut.Month)
        {
            var DateLocale = dateCourante;
            var unMois = new StatutMoisVM { Date = DateLocale };
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

            StatutsMois.Add(unMois);

            dateCourante = dateCourante.AddMonths(-1);
        }
    }

    public async Task GetFluxMensuel(DateTime date)
    {
        var dateDebut = new DateTime(date.Year, date.Month, 1);
        var dernierJourDuMois = DateTime.DaysInMonth(date.Year, date.Month);
        var dateFin = new DateTime(date.Year, date.Month, dernierJourDuMois);

        await _powensDataService.GetFlux(dateDebut, dateFin);
    }

    public async Task UpdateFluxMensuel()
    {
        if (FluxMensuel == null)
            throw new Exception("Aucune données mensuel");

        await _fluxCreditCoopService.UpdateFluxCreditCoopMensuel(FluxMensuel);
    }
}