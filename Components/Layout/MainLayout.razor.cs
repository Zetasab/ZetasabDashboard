using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Layout
{
    public partial class MainLayout
    {
        #region Injects
        [Inject] CommonServices CommonService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] NavigationManager Navigator { get; set; }
        [Inject] CommonServices CService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        #endregion

        #region Vars
        private UserModel LoggedUser { get; set; }
        private bool IsAuthentificated { get; set; } = false;
        #endregion

        #region LifeCycles
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsAuthentificated = await (AuthProvider as CustomAuthenticationStateProvider).IsAuthentificatedAsync();
                if (IsAuthentificated)
                {
                    LoggedUser = (AuthProvider as CustomAuthenticationStateProvider).LoggedUser;
                    await JS.InvokeVoidAsync("hideSplash");
                    StateHasChanged();
                }
                else
                {
                    Navigator.NavigateTo("/login");
                }
            }
        }
        #endregion

        #region Theme

        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        private MudTheme MyTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                PrimaryDarken = "463e8b"
            }
        };

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
        }
        #endregion

        #region LogOut
        private async Task LogOutClicked()
        {
            (AuthProvider as CustomAuthenticationStateProvider).Logout(Navigator);
        }
        #endregion
    }
}
