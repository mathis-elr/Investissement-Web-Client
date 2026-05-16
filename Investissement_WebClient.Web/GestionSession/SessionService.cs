using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Investissement_WebClient.Web.GestionSession;
public class SessionService(AuthenticationStateProvider authStateProvider)
{
    public int Id { get; private set; }
    public string Prenom { get; private set; } = string.Empty;
    public string AnneeCreation { get; private set; } = string.Empty;
    public bool IsConnected { get; private set; }

    public async Task Initialiser()
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is { IsAuthenticated: true })
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int id)) Id = id;

            Prenom = user.Identity.Name ?? "";
            AnneeCreation = user.FindFirst(ClaimTypes.DateOfBirth)?.Value ?? "";
            IsConnected = true;
        }
        else
        {
            Id = 0;
            Prenom = "";
            AnneeCreation = "";
            IsConnected = false;
        }
    }
}