namespace Investissement_WebClient.Application.Services.Powens;

public interface IPowensDataService
{
    Task GetToken(string code);

    Task GetFlux(DateTime dateDebut, DateTime dateFin);
}