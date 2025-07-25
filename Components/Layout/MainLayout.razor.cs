using MudBlazor;

namespace ZetaDashboard.Components.Layout
{
    public partial class MainLayout
    {
        private bool IsDarkMode = true; // ⬅️ Dark mode por defecto

        private string DarkModeIcon => IsDarkMode ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode;

        private MudTheme MyTheme = new MudTheme(); // Personalízalo si quieres

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
        }
    }
}
