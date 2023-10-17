using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomAuthentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsIdentity identity = new ClaimsIdentity(); // Initially empty.

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        public void MarkUserAsAuthenticated(string username)
        {
            identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "apiauth_type");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            identity = new ClaimsIdentity();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}