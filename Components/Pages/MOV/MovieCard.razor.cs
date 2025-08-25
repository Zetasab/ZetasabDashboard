using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV
{
    public partial class MovieCard
    {
        [Parameter]
        public MovieModel movie { get; set; }
        [Parameter] public DataController DController { get; set; }
        [Parameter] public UserModel LoggedUser { get; set; }
        [Parameter] public BaseService ApiService { get; set; }
        [Parameter] public CommonServices CService { get; set; }
        [Parameter] public List<MovieModel> SeenMovies { get; set; }
        [Parameter] public List<MovieModel> LikedMovies { get; set; }
        [Parameter] public List<MovieModel> WatchMovies { get; set; }
        [Parameter] public EventCallback Update { get; set; }

        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Admin
        };

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
        #region Like
        private async Task MarkAsLiked(MovieModel movie)
        {
            _= await DController.UpdateData(await ApiService.LikedMovies.MarkAsLikedAsync(movie,LoggedUser),
                LoggedUser,
                "MarkAsLiked",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateLikedList();
        }
        private async Task UnMarkAsLiked(MovieModel movie)
        {
            _ = DController.UpdateData(await ApiService.LikedMovies.UnMarkAsLikedAsync(movie,LoggedUser),
                LoggedUser,
                "UnMarkAsLiked",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateLikedList();
        }
        #endregion
        #region Watch
        private async Task MarkAsWatch(MovieModel movie)
        {
            _= await DController.UpdateData(await ApiService.WatchMovies.MarkAsWatchAsync(movie,LoggedUser),
                LoggedUser,
                "MarkAsWatch",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateWatchList();
        }
        private async Task UnMarkAsWatch(MovieModel movie)
        {
            _ = DController.UpdateData(await ApiService.WatchMovies.UnMarkAsWatchAsync(movie,LoggedUser),
                LoggedUser,
                "UnMarkAsWatch",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateWatchList();
        }
        #endregion

        private async Task UpdateSeenList()
        {
            SeenMovies = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            Update.InvokeAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task UpdateLikedList()
        {
            LikedMovies = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            Update.InvokeAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task UpdateWatchList()
        {
            WatchMovies = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            Update.InvokeAsync();
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
