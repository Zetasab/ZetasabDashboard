using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using ZetaCommon.Auth;

namespace ZetaDashboard.Components.Layout
{
    public partial class LoginLayout
    {
        [Inject] IJSRuntime JS { get; set; }

        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        private MudTheme MyTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                PrimaryDarken = "463e8b"
            }
        };

        #region LifeCycles
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("hideSplash");
                StateHasChanged();
            }
        }
        #endregion
    }
}
