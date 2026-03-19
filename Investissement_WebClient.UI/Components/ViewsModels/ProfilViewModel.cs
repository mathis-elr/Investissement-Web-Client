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
        public decimal EvolutionAnnuellePourcentage { get; set; }
        public int PerspectiveNbAnnees { get; set; }

        /* PROPRIETES INVESTISSEMENT */
        public decimal InvestissementMoyenMensuel {  get; set; }
        public decimal InvestissementTotal {  get; set; }
        public IEnumerable<ValeurParDate> InvestissementParMois {  get; set; }

        /* PROPRIETES REVENUS */
        public List<Revenu> Revenus { get; set; }  
        public decimal PartInvestissement { get; set; }


        /*  PROPRIETES EVOLUTION ACTIFS */
        public IEnumerable<EvolutionActifDTO> EvolutionActifs { get; set; }
    }
}
