using Investissement_WebClient.Application.DTO.Patrimoine;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Web.GestionSession;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels.CompteTradeRepublic.Onglets
{
    public class TradeRepublicAllocationViewModel(SessionService sessionService,
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


        public bool RecuparationEnCours { get; set; } = false;

        // REPARTITION ACTIFS
        public IEnumerable<TypeAllocation> TypeGraphiquesPossibles => Enum.GetValues<TypeAllocation>();
        public TypeAllocation TypeAllocationSelectionnee { get; set; } = TypeAllocation.ParActif;
        public IEnumerable<ValeurTotaleParActifDto> ProportionParActif { get; set; } = [];
        public IEnumerable<ValeurTotaleParActifDto> ProportionParTypeActif { get; set; } = [];
        public IEnumerable<ValeurTotaleParActifDto> ProportionParTypeCompte { get; set; } = [];
        public IEnumerable<ValeurTotaleParActifDto> ProportionParZone { get; set; } = [];

        public IEnumerable<ValeurTotaleParActifDto> ValeuresAllocationSelectionnee { get; set; } = [];


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
                     LoadProportionParActif(prixParActif)
                    );

                ValeuresAllocationSelectionnee = ProportionParActif;
            }
            finally
            {
                RecuparationEnCours = false;
            }
        }

        public async Task ChangerTypeAllocationSelectionnee(TypeAllocation typeAllocation)
        {
            if (TypeAllocationSelectionnee == typeAllocation || RecuparationEnCours)
                return;

            TypeAllocationSelectionnee = typeAllocation;

            switch (typeAllocation) 
            {
                case TypeAllocation.ParActif:
                    ValeuresAllocationSelectionnee = ProportionParActif;
                    break;
                case TypeAllocation.ParCompte:
                    ValeuresAllocationSelectionnee = ProportionParTypeCompte;
                    break;
                case TypeAllocation.ParType:
                    ValeuresAllocationSelectionnee = ProportionParTypeActif;
                    break;
                case TypeAllocation.ParZone:
                    ValeuresAllocationSelectionnee = ProportionParZone;
                    break;
            }

            NotifyStateChanged();
        }

        public string ToStringPourcentage(decimal valeur, string devise)
        {
            return valeur.ToString(devise, CultureInfo.GetCultureInfo("fr-FR"));
        }

        public string GetLibelleTypeAllocation(TypeAllocation periode)
        {
            return periode switch
            {
                TypeAllocation.ParActif => "Par actif",
                TypeAllocation.ParCompte => "Par compte",
                TypeAllocation.ParType => "Par type",
                TypeAllocation.ParZone => "Par zone",
                _ => string.Empty
            };
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

        private async Task LoadProportionParActif(Dictionary<string, decimal> prixParActif)
        {
            ProportionParActif = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }
        private async Task LoadProportionParTypeActif(Dictionary<string, decimal> prixParActif)
        {
            //ProportionParAcitfInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }

        private async Task LoadProportionParTypeCompte(Dictionary<string, decimal> prixParActif)
        {
            //ProportionParAcitfInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }

        private async Task LoadProportionParZone(Dictionary<string, decimal> prixParActif)
        {
            //ProportionParAcitfInvestit = await _fluxInvestissementService.GetValeurParActifInvestit(prixParActif, IdUser);
        }

    }
}
