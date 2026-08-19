using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Interfaces.APIs
{
    public interface IPowensApiService
    {
        Task CreeNouvelUtilisateur(int userId);

        Task VerifierUtilisateurPowensExists(int userId);

        Task<string> GenerateCodeTemporaireByUserId(int userId);

        Task SaveBanque(int connectionId, int userId);

        Task GetFlux(DateTime dateDebut, DateTime dateFin, CompteBanque compteBanque);
    }
}