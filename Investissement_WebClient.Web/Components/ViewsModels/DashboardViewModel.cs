using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Application.DTO.Profil;
using Investissement_WebClient.Web.GestionSession;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class DashboardViewModel(SessionService sessionService, 
                                 IFluxInvestissementService fluxInvestissementService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        // MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        // PROPRIETES PERSPECTIVES
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal EvolutionAnnuellePourcentage { get; set; } = 8;
        public int PerspectiveNbAnnees { get; set; } = 15;
        public List<ValeurParAnLineChartDto> PerspectivesValeurPatrimoineParAn { get; set; } = [];

        // INFOS COMPTE
        public bool RecuparationEnCours { get; set; } = false;
        public decimal ValeurPatrimoineCourante { get; set; }
        private decimal ValeurInvestissementTotal { get; set; }
        public decimal GainTotal => ValeurPatrimoineCourante - ValeurInvestissementTotal;
        public int NombreAnnes { get; set; }
        public int NombreMois { get; set; }
        public int NombreActifs => ValeurParActifInvestit.Count();

        // REPARTITION ACTIFS
        public IEnumerable<ValeurTotaleParActifDto> ValeurParActifInvestit { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;


        public async Task LoadData()
        {
            RecuparationEnCours = true;

            try
            {
                await _sessionService.VerifierInitialisation();
                IdUser = _sessionService.Id;
                PrenomUser = _sessionService.Prenom;

                var prixParActif = await _fluxInvestissementService.GetPrixParActif();

                await Task.WhenAll(
                     LoadValeurPatrimoineCourante(prixParActif),
                     LoadProportionParActif(prixParActif),
                     LoadInvestissementMoyenMensuel(),
                     LoadValeurInvestissementTotale(),
                     CalculerInvestisseurDepuis()
                    );

                CalculerEvolutionDuPatrimoine();
            }
            finally
            {
                RecuparationEnCours = false;
            }
        }

        public async Task LoadInfosProfil()
        {
            var prixParActif = await _fluxInvestissementService.GetPrixParActif();

            await Task.WhenAll(
                 LoadValeurPatrimoineCourante(prixParActif),
                 LoadValeurInvestissementTotale()
                );

            NotifyStateChanged();
        }

        public void CalculerEvolutionDuPatrimoine()
        {

            if (InvestissementMoyenMensuel < 1)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution d'un investissement null";
                return;
            }
            if (EvolutionAnnuellePourcentage < 1)
            {
                HasError = true;
                ErrorMessage = "Entrez une évolution annuelle positive";
                return;
            }
            if (PerspectiveNbAnnees < 1)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution de moins d'une année";
                return;
            }
            if (PerspectiveNbAnnees > 80)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution pour plus de 80 ans";
                return;
            }
            if (EvolutionAnnuellePourcentage > 50)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution pour plus de 50% d'évolution par an";
                return;
            }
            if (InvestissementMoyenMensuel > 100_000)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution pour plus de 100k d'investissement mensuel";
                return;
            }

            PerspectivesValeurPatrimoineParAn = [];
            int annee = 0;
            decimal investissementCourant = 0;
            decimal pourcentageAnnuel = 1 + (EvolutionAnnuellePourcentage / 100);
            double pourcentageMensuel = Math.Pow((double)pourcentageAnnuel, 1.0 / 12);
            double pointFixe = (double)InvestissementMoyenMensuel / (pourcentageMensuel - 1);

            while (annee <= PerspectiveNbAnnees)
            {
                var valeurDouble = pointFixe * (Math.Pow(pourcentageMensuel, annee * 12) - 1);

                if (double.IsInfinity(valeurDouble) || valeurDouble > (double)decimal.MaxValue)
                {
                    valeurDouble = (double)decimal.MaxValue;
                }

                var valeurParAn = new ValeurParAnLineChartDto
                {
                    Annee = annee,
                    Valeur = (decimal)Math.Round(valeurDouble, 0),
                    Investissement = investissementCourant
                };

                PerspectivesValeurPatrimoineParAn.Add(valeurParAn);
                annee++;
                investissementCourant += InvestissementMoyenMensuel * 12;
            }
        }

        public string ToStringPourcentage(decimal valeur, string devise)
        {
            return valeur.ToString(devise, CultureInfo.GetCultureInfo("fr-FR"));
        }

        public string DeterminerClasse(decimal variationPrix)
        {
            return variationPrix switch
            {
                > 0 => "vert",
                < 0 => "rouge",
                _ => "blanc"
            };
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _fluxInvestissementService.CalculerInvestissementMedianMensuel(IdUser);
            if (InvestissementMoyenMensuel == 0) InvestissementMoyenMensuel = 100;
        }

        private async Task CalculerInvestisseurDepuis()
        {
            var datePremierInvest = await _fluxInvestissementService.GetDatePremierFlux(IdUser);
            if (datePremierInvest.HasValue)
            {
                var nbJours = (DateTime.Today - datePremierInvest.Value.Date).Days;
                NombreAnnes = nbJours / 365;
                NombreMois = (nbJours % 365) / 30;
            }
        }

        private async Task LoadValeurPatrimoineCourante(Dictionary<string, decimal> prixParActif)
        {
            try
            {
                ValeurPatrimoineCourante = await _fluxInvestissementService.CalculerValeurCourante(prixParActif, IdUser);
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

        private async Task LoadProportionParActif(Dictionary<string, decimal> prixParActif)
        {
            ValeurParActifInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }
    }
}
