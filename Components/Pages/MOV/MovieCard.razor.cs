using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.MOV;

namespace ZetaDashboard.Components.Pages.MOV
{
    public partial class MovieCard
    {
        [Parameter]
        public MovieModel movie { get; set; }
        private Color GetScoreColor(double voteAverage)
        {
            var percent = voteAverage * 10; // 0-100
            if (percent >= 70) return Color.Success;   // verde
            if (percent >= 40) return Color.Warning;   // amarillo
            return Color.Error;                        // rojo
        }

    }
}
