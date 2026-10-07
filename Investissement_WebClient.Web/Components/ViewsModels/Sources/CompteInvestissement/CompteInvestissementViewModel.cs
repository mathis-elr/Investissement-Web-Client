using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Domain.Enums;
using Investissement_WebClient.Web.GestionSession;
using System.Globalization;

namespace Investissement_WebClient.Web.Components.ViewsModels.Sources.CompteInvestissement
{
    public class CompteInvestissementViewModel(IPositionInvestissementService positionInvestissementService,
                                               IPowensApiService powensApiService,
                                               SessionService sessionService)
    {
        private readonly IPositionInvestissementService _positionInvestissementService = positionInvestissementService;
        private readonly IPowensApiService _powensApiService = powensApiService;
        private readonly SessionService _sessionService = sessionService;

        public SourceDto CompteInvestissementCourant { get; set; } = null!;

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        //MAJ VUE
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public bool ActionEnCours { get; set; } = false;
        public bool SynchronisationEnCours { get; set; } = false;

        public string UrlReconnectionPowens { get; set; } = string.Empty;

        // DATAS
        public IEnumerable<PositionInvestissementDto> PositionsInvestissement { get; set; } = [];
        public string StatusSynchronisation { get; set; } = string.Empty;
        public string BadgeSynchronisation { get; set; } = string.Empty;
        public decimal ValeureTotale => PositionsInvestissement.Sum(p => p.Quantite * p.PrixCourant);
        public decimal ValeureInvesti => PositionsInvestissement.Sum(p => p.Quantite * p.PrixAchat);
        public decimal VariationPourcentage => ValeureTotale == 0 ? 0 : (ValeureTotale - ValeureInvesti) / ValeureInvesti;

        // GESTION ERREUR
        public bool HasError { get; set; } = false;
        public string MessageError { get; set; } = string.Empty;

        public async Task StartLoadData(SourceDto sourceSelectionnee)
        {
            ActionEnCours = true;

            try
            {
                await InitialiserSession();

                CompteInvestissementCourant = sourceSelectionnee;

                await LoadPositions();
                await SetUrlReconnection();
                DeterminerStatutSynchronisation();
            }
            finally
            {
                ActionEnCours = false;
            }
        }

        public void DeterminerStatutSynchronisation()
        {
            var compte = CompteInvestissementCourant;

            if (!compte.DerniereSychro.HasValue)
            {
                BadgeSynchronisation = "non-synchronise";
                StatusSynchronisation = "Non synchronisé";
                return;
            }

            if (compte.StatutConnexion != StatutConnexion.Valide)
            {
                BadgeSynchronisation = "synchro-requise";
                StatusSynchronisation = "Reconnexion requise";
                return;
            }

            BadgeSynchronisation = "synchronise";
            var local = compte.DerniereSychro.Value.ToLocalTime();
            var format = local.Date == DateTime.Today ? "aujourd'hui à HH:mm" : "le dd MMM à HH:mm";

            StatusSynchronisation = $"Synchronisé {local.ToString(format)}";
        }

        public async Task SynchroniserPositions()
        {
            SynchronisationEnCours = true;

            try
            {
                await _powensApiService.GetPositionInvestissements(CompteInvestissementCourant.Id);
            }
            catch (Exception ex)
            {
                HasError = true;
                MessageError = ex.Message;
            }
            finally 
            { 
                SynchronisationEnCours = false;
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

        private async Task SetUrlReconnection()
        {
            UrlReconnectionPowens = await _powensApiService.GetUrlReconnexionPowens(IdUser, CompteInvestissementCourant.Id);
        }

        private async Task InitialiserSession()
        {
            await _sessionService.VerifierInitialisation();
            IdUser = _sessionService.Id;
        }

        private async Task LoadPositions()
        {
            PositionsInvestissement = await _positionInvestissementService.GetPositionsByCompte(CompteInvestissementCourant.Id);
        }
    }
}
