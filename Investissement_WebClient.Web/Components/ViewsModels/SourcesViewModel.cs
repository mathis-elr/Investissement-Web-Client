using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Infrastructure.APIs.Powens;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.Extensions.Options;

namespace Investissement_WebClient.Web.Components.ViewsModels
{
    public class SourcesViewModel(ISourceRepository sourceRepository,
                                  IOptions<PowensApiOptions> options,
                                  IPowensApiService powensApiService,
                                  SessionService sessionService)
    {
        private readonly ISourceRepository _sourceRepository = sourceRepository;
        private readonly IPowensApiService _powensApiService = powensApiService;
        private readonly PowensApiOptions _powensApiOptions = options.Value;
        private readonly SessionService _sessionService = sessionService;

        // CONNEXION BANQUE
        public string UrlConnexionPowens { get; set; } = string.Empty;
        public List<Source> Sources { get; set; } = [];
        public bool AucuneSource => Sources.Count == 0;
        public Source? SourceSelectionne { get; set; }

        // USER CONNECTE
        public int IdUser { get; set; }
        public string PrenomUser { get; set; } = string.Empty;

        //MAJ VUE
        public bool Chargement { get; set; } = false;
        public event Action OnChange = null!;
        public void NotifyStateChanged() => OnChange?.Invoke();

        // GESTION D'ERREUR
        public string MessageErreur { get; set; } = string.Empty;
        public bool HasErreur { get; set; } = false;


        public async Task StartLoadData()
        {
            Chargement = true;

            try
            {
                await InitialiserSession();

                await LoadComptesBanque();

                if (AucuneSource)
                    return;

                SourceSelectionne = Sources.First();
            }
            finally
            {
                Chargement = false;
            }
        }

        public async Task FinaliserAjoutBanque(int connectionBanqueId)
        {
            await InitialiserSession();

            try
            {
                await _powensApiService.SaveBanque(connectionBanqueId, IdUser);
            }
            catch (Exception ex)
            {
                HasErreur = true;
                MessageErreur = ex.Message;
            }
        }     

        public async Task InitialiserUrlConnexionPowens()
        {
            UrlConnexionPowens = await GetUrlConnexionPowens();
        }

        public async Task ChangerSourceSelectionne(Source source)
        {
            SourceSelectionne = source;
        }

        private async Task InitialiserSession()
        {
            await _sessionService.Initialiser();
            IdUser = _sessionService.Id;
        }

        private async Task LoadComptesBanque()
        {
            Sources = await _sourceRepository.GetAllByUserId(IdUser);
        } 

        private async Task<string> GetUrlConnexionPowens()
        {
            await _powensApiService.VerifierUtilisateurPowensExists(IdUser);

            var code = await _powensApiService.GenerateCodeTemporaireByUserId(IdUser);

            var fullConnectUrl = new Uri(new Uri(_powensApiOptions.BaseUri), _powensApiOptions.ConnectEndPoint);

            var encodedRedirect =
                Uri.EscapeDataString(_powensApiOptions.RedirectUri);

            return $"{fullConnectUrl}?client_id={_powensApiOptions.ClientId}&redirect_uri={encodedRedirect}&code={code}";
        }
    }
}