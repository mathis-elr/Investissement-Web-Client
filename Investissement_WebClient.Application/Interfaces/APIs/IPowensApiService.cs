using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.APIs
{
    public interface IPowensApiService
    {
        Task<string> GetUrlConnexionPowens(int idUser);

        Task<string> GetUrlReconnexionPowens(int idUser, int compteId);

        Task CreeNouvelUtilisateur(int userId);

        Task VerifierUtilisateurPowensExists(int userId);

        Task<string> GenerateCodeTemporaireByUserId(int userId);

        Task SaveBanque(int connectionId, int userId);

        Task VerifierEtSynchroniserFluxBancairesAsync();

        Task GetTransactions(DateTime dateDebut, DateTime dateFin, CompteBanque compteBanque);

        Task GetPositionInvestissements(CompteBanque compteBanque);

        Task SynchroniserSoldeComptes();
    }
}