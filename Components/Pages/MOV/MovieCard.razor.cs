using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Pages.MOV
{
    public partial class MovieCard
    {
        [Parameter]
        public MovieModel movie { get; set; }
        [Parameter] public DataController DController { get; set; }
        [Parameter] public UserModel LoggedUser { get; set; }
        [Parameter] public BaseService ApiService { get; set; }
        [Parameter] public List<MovieModel> SeenMovies { get; set; }


        #region Seen
        private async Task MarkAsSeen(MovieModel movie)
        {
            _= await DController.UpdateData(await ApiService.SeenMovies.MarkAsSeenAsync(movie,LoggedUser),
                LoggedUser,
                "MarkAsSeen",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateSeenList();
        }
        private async Task UnMarkAsSeen(MovieModel movie)
        {
            _ = DController.UpdateData(await ApiService.SeenMovies.UnMarkAsSeenAsync(movie,LoggedUser),
                LoggedUser,
                "UnMarkAsSeen",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateSeenList();
        }
        #endregion

        private async Task UpdateSeenList()
        {
            SeenMovies = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            await InvokeAsync(StateHasChanged);
        }

        private Color GetScoreColor(double voteAverage)
        {
            var percent = voteAverage * 10; // 0-100
            if (percent >= 70) return Color.Success;   // verde
            if (percent >= 40) return Color.Warning;   // amarillo
            return Color.Error;                        // rojo
        }

    }
}
