using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Layout
{
    public partial class MainLayout
    {
        #region Injects
        [Inject] CommonServices CommonService { get; set; }
        [Inject] BaseService ApiService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] NavigationManager Navigator { get; set; }
        [Inject] CommonServices CService { get; set; }
        [Inject] IJSRuntime JS { get; set; }
        #endregion

        #region Vars
        private UserModel LoggedUser { get; set; }
        private bool IsAuthentificated { get; set; } = false;
        #endregion
        DotNetObjectReference<MainLayout>? _ref;
        #region LifeCycles
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsAuthentificated = await (AuthProvider as CustomAuthenticationStateProvider).IsAuthentificatedAsync();
                if (IsAuthentificated)
                {
                    LoggedUser = (AuthProvider as CustomAuthenticationStateProvider).LoggedUser;
                    MyTheme = await GetTheme();
                    await JS.InvokeVoidAsync("hideSplash");
                    _ref = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("clientErrorTap.init", _ref);
                    StateHasChanged();
                }
                else
                {
                    Navigator.NavigateTo("/login");
                }
            }
        }
        #endregion
        [JSInvokable]
        public Task ReportClientError(string msg)
        {
            Console.WriteLine($"[ClientError][iOS] {msg}");
            // TODO: también podrías guardar en Mongo para ver histórico
            return Task.CompletedTask;
        }

        public void Dispose() => _ref?.Dispose();

        #region Theme

        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        

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

        #region Theme

        private async Task<MudTheme> GetTheme()
        {
            var theme = LoggedUser.MudCustomTheme switch
            {
                "default" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        PrimaryDarken = "463E8B"
                    }
                },
                "purple" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        PrimaryDarken = "463E8B"
                    }
                },
                "blue" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        Primary = "064984", // azul vivo
                    }
                },
                "green" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        Primary = "055e2a", // verde menta
                    }
                },
                "orange" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        Primary = "7a3823", // naranja pastel moderno
                    }
                },
                "red" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        Primary = "661e1d", // rojo vivo coral
                    }
                },
                "yellow" => new MudTheme()
                {
                    PaletteDark = new PaletteDark()
                    {
                        Primary = "6e5310", // dorado cálido
                    }
                },
            };

            return theme;
        }
        //public MudTheme MyTheme = new MudTheme()
        //{
        //    PaletteDark = new PaletteDark()
        //    {
        //        PrimaryDarken = "463e8b"
        //        //PrimaryDarken = "ffffff"
        //    }
        //};

        public MudTheme MyTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = "463E8B",
            }
        };
        #endregion

       
    }
}
