using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Core.Modeles;
using Investissement_WebClient.Core.Modeles.DTO;
using Investissement_WebClient.Core.Modeles.Graphiques;

namespace Investissement_WebClient.UI.Components.ViewsModels
{
    public class ProfilViewModel
    {
        private readonly IPatrimoineService _patrimoineService;
        private readonly IInvestirService _investirService;

        public ProfilViewModel(IPatrimoineService patrimoineService, IInvestirService investirService)
        {
            _patrimoineService = patrimoineService;
            _investirService = investirService;
        }

        /*  PROPRIETES PERSPECTIVES */
        public double EvolutionAnnuellePourcentage { get; set; } = 8;
        public int PerspectiveNbAnnees { get; set; } = 10;
        public List<ValeurParAn> PerspectivesValeurPatrimoineParAn { get; set; } = [];

        /* PROPRIETES INVESTISSEMENT */
        public double InvestissementMoyenMensuel { get; set; } = 800;
        public decimal InvestissementTotal {  get; set; }
        public IEnumerable<ValeurParDate> InvestissementParMois {  get; set; }

        /* PROPRIETES REVENUS */
        public List<Revenu> Revenus { get; set; }  
        public decimal PartInvestissement { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }


        public void CalculerEvolutionDuPatrimoine()
        {
            PerspectivesValeurPatrimoineParAn = [];

            int annee = 0;
            double investissementCourant = 0;
            double pourcentageAnnuel = 1 + (EvolutionAnnuellePourcentage / 100);
            double pourcentageMensuel = Math.Pow(pourcentageAnnuel, 1.0 / 12);
            double pointFixe = InvestissementMoyenMensuel / (pourcentageMensuel - 1);

            while (annee <= PerspectiveNbAnnees)
            {
                var valeurParAn = new ValeurParAn
                {
                    Annee = annee,
                    Valeur = (decimal)Math.Round(pointFixe * (Math.Pow(pourcentageMensuel, annee * 12) - 1), 0),
                    Investissement = (decimal)investissementCourant
                };
                
                PerspectivesValeurPatrimoineParAn.Add(valeurParAn);
                annee++;
                investissementCourant += InvestissementMoyenMensuel * 12;
            }
        }
    }
}
