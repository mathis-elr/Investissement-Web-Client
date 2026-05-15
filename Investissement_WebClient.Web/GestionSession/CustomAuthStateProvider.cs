using Investissement_WebClient.Web.GestionSession;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Investissement_WebClient.Web.GestionSession;
public class CustomAuthStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // On tente de lire le stockage
            var result = await localStorage.GetAsync<SessionUtilisateur>("UserSession");

            if (!result.Success || result.Value == null)
                return new AuthenticationState(_anonymous);

            var userSession = result.Value;
            var claimsPrincipal = CreateClaimsPrincipal(userSession);

            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            // Si JS Interop n'est pas encore prêt (cas rare avec prerender:false mais arrive)
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task UpdateAuthenticationState(SessionUtilisateur? userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession != null)
        {
            await localStorage.SetAsync("UserSession", userSession);
            claimsPrincipal = CreateClaimsPrincipal(userSession);
        }
        else
        {
            await localStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    private ClaimsPrincipal CreateClaimsPrincipal(SessionUtilisateur session)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.Id.ToString()),
            new Claim(ClaimTypes.Email, session.Email),
            new Claim(ClaimTypes.Name, session.Prenom)
        }, "CustomAuth"));
    }
}