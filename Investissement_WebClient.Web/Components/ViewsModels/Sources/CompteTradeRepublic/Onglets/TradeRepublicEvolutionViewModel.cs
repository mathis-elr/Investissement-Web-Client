using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Domain.Enums;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels.CompteTradeRepublic.Onglets
{
    public class TradeRepublicEvolutionViewModel(SessionService sessionService,
                                                 IValeurPatrimoineService valeurPatrimoineService,
                                                 IFluxInvestissementService fluxInvestissementService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IValeurPatrimoineService _valeurPatrimoineService = valeurPatrimoineService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;


        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        // MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        // DATAS INFOS PATRIMOINE
        public bool RecuparationEnCours { get; set; } = false;
        public decimal ValeurPatrimoineCourante { get; set; }
        public bool AucuneDonnees { get; set; } = false;
        private decimal ValeurInvestissementTotal { get; set; }
        public decimal GainTotal => ValeurPatrimoineCourante - ValeurInvestissementTotal;
        public IEnumerable<VariationDto> Variations { get; set; } = [];
        public VariationDto? VariationSelectionnee => Variations.FirstOrDefault(v => v.Periode == PeriodeSelectionnee);

        // DATAS GRAPHIQUES
        public IEnumerable<BougieChartDto> BougiesPlusValue { get; set; } = [];
        public IEnumerable<PointChartDto> PointsPlusValue { get; set; } = [];
        public IEnumerable<Periode> PeriodesPossibles => Enum.GetValues<Periode>();
        public IEnumerable<Granulometrie> GranulometriesPossibles => Enum.GetValues<Granulometrie>();
        public IEnumerable<TypeGraphique> TypeGraphiquesPossibles => Enum.GetValues<TypeGraphique>();
        public Periode PeriodeSelectionnee { get; set; } = Periode.Tout;
        public Granulometrie GranulometrieSelectionnee { get; set; } = Granulometrie.Journalier;
        public TypeGraphique TypeGraphiqueSelectionnee { get; set; } = TypeGraphique.Line;

        public IEnumerable<BougieChartDto> BougiesJournalieresValeurPatrimoineSurInvestissementTotal { get; set; } = [];
        public IEnumerable<ValeurTotaleParActifDto> ValeurParActifInvestit { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;


        public async Task StartLoadData()
        {
            RecuparationEnCours = true;

            try
            {
                await _sessionService.VerifierInitialisation();
                IdUser = _sessionService.Id;

                var prixParActif = await _fluxInvestissementService.GetPrixParActif();

                await LoadValeurPatrimoineCourante(prixParActif);

                if (!AucuneDonnees)
                {
                    await LoadValeurInvestissementTotale();

                    await Task.WhenAll(
                        LoadVariationsPrix(),
                        LoadBougiesPlusValue(),
                        LoadPointsPlusValue(),
                        LoadProportionParActif(prixParActif),
                        LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal()
                    );
                }
            }
            finally
            {
                RecuparationEnCours = false;
            }
        }

        public string DeterminerClasse(decimal variationPrix)
        {
            return variationPrix switch
            {
                > 0 => "vert",
                < 0 => "rouge",
                _ => "gris"
            };
        }

        public string ToStringPourcentage(decimal valeur, string devise)
        {
            return valeur.ToString(devise, CultureInfo.GetCultureInfo("fr-FR"));
        }

        public string GetLibelleGranulometrie(Granulometrie granulometrie)
        {
            return granulometrie switch
            {
                Granulometrie.Journalier => "J",
                Granulometrie.Hebdomadaire => "H",
                Granulometrie.Mensuel => "M",
                _ => string.Empty
            };
        }

        public string GetLibellePeriode(Periode periode)
        {
            return periode switch
            {
                Periode.Jour => "24H",
                Periode.Semaine => "1S",
                Periode.Mois => "1M",
                Periode.SixMois => "6M",
                Periode.Ans => "1A",
                Periode.CinqAns => "5A",
                Periode.Tout => "Tout",
                _ => string.Empty
            };
        }

        public string GetIconeTypeGraphique(TypeGraphique typeGraphique, bool actif)
        {
            return typeGraphique switch
            {
                TypeGraphique.Candle =>
                    actif
                        ? "/icons/candle-orange.svg"
                        : "/icons/candle-white.svg",

                TypeGraphique.Line =>
                    actif
                        ? "/icons/line-orange.svg"
                        : "/icons/line-white.svg",

                _ => string.Empty
            };
        }

        public async Task ChangerPeriodeSelectionnee(Periode periode)
        {
            if (PeriodeSelectionnee == periode || RecuparationEnCours)
                return;

            PeriodeSelectionnee = periode;

            await RafraichirGraphique();
        }

        public async Task ChangerGranulometrieSelectionnee(Granulometrie granulometrie)
        {
            if (GranulometrieSelectionnee == granulometrie || RecuparationEnCours)
                return;

            GranulometrieSelectionnee = granulometrie;

            await RafraichirGraphique();
        }

        public async Task ChangerTypeGraphiqueSelectionnee(TypeGraphique typeGraphique)
        {
            if (TypeGraphiqueSelectionnee == typeGraphique || RecuparationEnCours)
                return;

            TypeGraphiqueSelectionnee = typeGraphique;

            await RafraichirGraphique();
        }

        private async Task RafraichirGraphique()
        {
            RecuparationEnCours = true;

            try
            {
                await Task.WhenAll(
                    LoadBougiesPlusValue(),
                    LoadPointsPlusValue()
                );

                NotifyStateChanged();
            }
            finally
            {
                RecuparationEnCours = false;
            }
        }

        private async Task LoadValeurPatrimoineCourante(Dictionary<string, decimal> prixParActif)
        {
            try
            {
                ValeurPatrimoineCourante = await _fluxInvestissementService.CalculerValeurCourante(prixParActif, IdUser);
                AucuneDonnees = ValeurPatrimoineCourante == 0;

            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadValeurInvestissementTotale()
        {
            ValeurInvestissementTotal = await _fluxInvestissementService.CalculerValeurInvestissementTotal(IdUser);
        }

        private async Task LoadVariationsPrix()
        {
            if (ValeurPatrimoineCourante == 0) return;
            Variations = await _valeurPatrimoineService.GetVariations(ValeurPatrimoineCourante, ValeurInvestissementTotal, IdUser);
        }

        private async Task LoadBougiesPlusValue()
        {
            BougiesPlusValue = await _valeurPatrimoineService.GetBougiesPlusValueByUserId(PeriodeSelectionnee, GranulometrieSelectionnee, IdUser);
        }

        private async Task LoadPointsPlusValue()
        {
            PointsPlusValue = await _valeurPatrimoineService.GetPointsPlusValueByUserId(PeriodeSelectionnee, GranulometrieSelectionnee, IdUser);
        }

        private async Task LoadBougiesJournalieresValeurPatrimoineSurInvestissementTotal()
        {
            BougiesJournalieresValeurPatrimoineSurInvestissementTotal = await _valeurPatrimoineService.GetBougiesJournalieresValeurPatrimoineSurInvestissmentTotal(IdUser);
        }

        private async Task LoadProportionParActif(Dictionary<string, decimal> prixParActif)
        {
            ValeurParActifInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }
    }
}
