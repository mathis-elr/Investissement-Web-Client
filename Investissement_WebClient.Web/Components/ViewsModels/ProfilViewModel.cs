using Investissement_WebClient.Application.Services.Investissement;
using Investissement_WebClient.Application.ViewsModels.Graphiques;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class ProfilViewModel(IInvestissementService investissementService)
    {
        private readonly IInvestissementService _investissementService = investissementService;
        
        // PROPRIETES PERSPECTIVES
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal EvolutionAnnuellePourcentage { get; set; } = 8;
        public int PerspectiveNbAnnees { get; set; } = 15;
        public List<ValeurParAnVM> PerspectivesValeurPatrimoineParAn { get; set; } = [];

        // GESTION D'ERREUR
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;


        public async Task LoadData()
        {
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
                var valeurParAn = new ValeurParAnVM
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

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _investissementService.CalculerInvestissementMoyenMensuel();
        }
    }
}
