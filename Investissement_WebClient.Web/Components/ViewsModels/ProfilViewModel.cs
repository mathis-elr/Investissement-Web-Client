using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.TradeRepublicApi;
using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Application.ViewsModels.Graphiques.Profils;
using Investissement_WebClient.Web.GestionSession;


namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class ProfilViewModel(SessionService sessionService, 
                                 IFluxInvestissementService fluxInvestissementService, 
                                 ITradeRepublicApiService tradeRepublicApiService)
    {
        private readonly SessionService _sessionService = sessionService;
        private readonly IFluxInvestissementService _fluxInvestissementService = fluxInvestissementService;
        private readonly ITradeRepublicApiService _tradeRepublicApiService = tradeRepublicApiService;

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;
        
        // PROPRIETES PERSPECTIVES
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal EvolutionAnnuellePourcentage { get; set; } = 8;
        public int PerspectiveNbAnnees { get; set; } = 15;
        public List<ValeurParAnLineChartVM> PerspectivesValeurPatrimoineParAn { get; set; } = [];

        // CONNECTION TR
        public TradeRepublicAccesVM TradeRepublicAcces { get; set; } = new TradeRepublicAccesVM();

        // GESTION D'ERREUR
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;


        public async Task LoadData()
        {
            await _sessionService.Initialiser();

            IdUser = _sessionService.Id;
            PrenomUser = _sessionService.Prenom;

            await LoadInvestissementMoyenMensuel();
            await CalculerEvolutionDuPatrimoine();
        }

        public async Task CalculerEvolutionDuPatrimoine()
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
            if (PerspectiveNbAnnees > 100)
            {
                HasError = true;
                ErrorMessage = "Impossible de calculer l'evolution pour plus de 100 ans";
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
                var valeurParAn = new ValeurParAnLineChartVM
                {
                    Annee = annee,
                    Valeur = (decimal)Math.Round(pointFixe * (Math.Pow(pourcentageMensuel, annee * 12) - 1), 0),
                    Investissement = investissementCourant
                };

                PerspectivesValeurPatrimoineParAn.Add(valeurParAn);
                annee++;
                investissementCourant += InvestissementMoyenMensuel * 12;
            }
        }

        public async Task SaveAccesTR()
        {
            try
            {
                await _tradeRepublicApiService.SaveAcces(TradeRepublicAcces, IdUser);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _fluxInvestissementService.CalculerInvestissementMoyenMensuel(IdUser);
            if (InvestissementMoyenMensuel == 0) InvestissementMoyenMensuel = 100;
        }
    }
}
