using MudBlazor;

namespace ZetaDashboard.Services
{
    public class CommonServices
    {

        #region Color
        public enum ColorType
        {
            Normal,
            Darken,
            Lighten
        }
        public string GetCssVar(Color color, ColorType colorType = ColorType.Normal)
        {
            var colorToReturn = color switch
            {
                Color.Primary => "var(--mud-palette-primary",
                Color.Secondary => "var(--mud-palette-secondary",
                Color.Tertiary => "var(--mud-palette-tertiary",
                Color.Info => "var(--mud-palette-info",
                Color.Success => "var(--mud-palette-success",
                Color.Warning => "var(--mud-palette-warning",
                Color.Error => "var(--mud-palette-error",
                Color.Dark => "var(--mud-palette-dark",
                _ => "inherit"
            };
            colorToReturn = colorToReturn + colorType switch
            {
                ColorType.Normal => ")",
                ColorType.Darken => "-darken)",
                ColorType.Lighten => "-lighten)",
                _ => ")"
            };

            return colorToReturn;
        }
        #endregion
    }
}
