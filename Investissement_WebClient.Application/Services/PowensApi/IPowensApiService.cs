namespace Investissement_WebClient.Application.Services.PowensApi;

public interface IPowensApiService
{
    Task<bool> ConnexionRequise();

    Task GetToken(string code);

    Task GetFlux(DateTime dateDebut, DateTime dateFin);
}