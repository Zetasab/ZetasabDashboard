using MudBlazor;

namespace ZetaDashboard.Components.Layout
{
    public partial class LoginLayout
    {
        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        private MudTheme MyTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                PrimaryDarken = "463e8b"
            }
        };
    }
}
