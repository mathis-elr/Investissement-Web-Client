namespace Investissement_WebClient.Application.Interfaces.APIs
{
    public interface IPowensApiService
    {
        Task GetToken(string code, int userId);

        Task GetFlux(DateTime dateDebut, DateTime dateFin, int userId);
    }
}