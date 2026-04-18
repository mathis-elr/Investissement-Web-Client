using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ProfilViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestissementService _investissementService;
        
        /*  PROPRIETES PERSPECTIVES */
        public decimal InvestissementMoyenMensuel { get; set; }
        public decimal EvolutionAnnuellePourcentage { get; set; }
        public int PerspectiveNbAnnees { get; set; }
        public List<ValeurParAn> PerspectivesValeurPatrimoineParAn { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public ProfilViewModel(IPatrimoineService patrimoineService, IInvestissementService investissementService)
        {
            _patrimoineService = patrimoineService;
            _investissementService = investissementService;
            
            EvolutionAnnuellePourcentage = 8;
            PerspectiveNbAnnees = 15;
            PerspectivesValeurPatrimoineParAn = [];
            
            HasError = false;
            ErrorMessage = string.Empty;
        }

        private async Task LoadInvestissementMoyenMensuel()
        {
            InvestissementMoyenMensuel = await _investissementService.CalculerInvestissementMoyenMensuel();
        }

        public async Task LoadData()
        {
            await LoadInvestissementMoyenMensuel();
            CalculerEvolutionDuPatrimoine();
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
                var valeurParAn = new ValeurParAn
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
    }
}
