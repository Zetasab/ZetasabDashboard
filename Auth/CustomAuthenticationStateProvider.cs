using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.Security.Claims;
using ZetaDashboard.Common.ZDB.Models;

namespace ZetaCommon.Auth
{
    public class CustomAuthenticationStateProvider(ProtectedSessionStorage storage) : AuthenticationStateProvider
    {
        public UserModel? LoggedUser { get; set; }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            
            try
            {
                var storageUser = await storage.GetAsync<UserModel>("user");

                if (storageUser.Success && storageUser.Value != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Name, storageUser.Value.Name),
            }, "apiauth");

                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }

            }
            catch (Exception ex)
            {

            }
                // Aquí ya no estamos en prerender, podemos usar ProtectedBrowserStorage

            // Durante el prerender, devolvemos un usuario anónimo
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task<bool> IsAuthentificatedAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            return authState.User.Identity.IsAuthenticated;
        }

        public async Task CorrectLogin(UserModel loggedUser)
        {
            LoggedUser = loggedUser;
            var identity = new ClaimsIdentity(new[]
                {new Claim(ClaimTypes.Name, loggedUser.Name)}, "dbauthType");
            var userClaim = new ClaimsPrincipal(identity);
             await storage.SetAsync("user", loggedUser);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userClaim)));


        }

        public async void Logout(NavigationManager Navigator)
        {
            try
            {
                LoggedUser = null;
                await storage.DeleteAsync("user");
                Navigator.NavigateTo("/login");
            }
            catch (Exception ex)
            {

            }
        }

    }
}
