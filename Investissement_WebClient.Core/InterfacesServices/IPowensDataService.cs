namespace Investissement_WebClient.Core.InterfacesServices;

public interface IPowensDataService
{
    Task GetToken(string code);

    Task GetFlux(DateTime dateDebut, DateTime dateFin);
}