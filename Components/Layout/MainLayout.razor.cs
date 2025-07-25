using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Layout
{
    public partial class MainLayout
    {
        #region Injects
        [Inject] CommonServices CommonService { get; set; }
        #endregion

        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        private MudTheme MyTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                PrimaryDarken = "463e8b"
            }
        }; // Personalízalo si quieres

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
        }
    }
}
