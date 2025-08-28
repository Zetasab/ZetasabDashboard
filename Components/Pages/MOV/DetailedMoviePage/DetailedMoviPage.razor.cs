using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZetaCommon.Auth;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV.DetailedMoviePage
{
    public partial class DetailedMoviPage
    {
        [Parameter] public string movieId { get; set; } = "";

        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
        #endregion
        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }
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
        #endregion

        private DetailedMovieModel MovieModel { get; set; } = new DetailedMovieModel();
        private MovieCreditsModel MovieCreditsModel { get; set; } = new MovieCreditsModel();
        private VideoResult MovieVideosModel { get; set; } = new VideoResult();
        private MovieListModel SimilarVideosModel { get; set; } = new MovieListModel();
        private MovieListModel Recomendated { get; set; } = new MovieListModel();
        private List<MovieModel> SeenMovies { get; set; } = new List<MovieModel>();
        private List<MovieModel> LikedMovies { get; set; } = new List<MovieModel>();
        private List<MovieModel> WatchMovies { get; set; } = new List<MovieModel>();

        private bool IsLoading = true;

        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en movie",
                $"Entrando en movie; movie {movieId}",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);

        }
        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            InvokeAsync(StateHasChanged);
            GetMovie();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        #endregion

        #region Crud
        #region Get
        private async Task GetMovie()
        {
            MovieModel = await DController.GetData(await HttpApiService.Movies.GetMovieByIdAsync(movieId, LoggedUser));
            MovieCreditsModel = await DController.GetData(await HttpApiService.Movies.GetCastMovieByIdAsync(movieId, LoggedUser));
            var video = new MovieVideosModel();
            video = await DController.GetData(await HttpApiService.Movies.GetVideoMovieByIdAsync(movieId, LoggedUser));
            MovieVideosModel = video.Results.FirstOrDefault(x => x.Type == "Teaser");
            SimilarVideosModel = await DController.GetData(await HttpApiService.Movies.GetSimilarMovieByIdAsync(movieId, LoggedUser));
            Recomendated = await DController.GetData(await HttpApiService.Movies.GetRecomendatedMovieByIdAsync(movieId, LoggedUser));


            SeenMovies = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            LikedMovies = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            WatchMovies = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();

            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        #endregion
        #endregion

        #region Seen
        private async Task MarkAsSeen(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = await DController.UpdateData(await ApiService.SeenMovies.MarkAsSeenAsync(moviee, LoggedUser),
                LoggedUser,
                "MarkAsSeen",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateSeenList();
        }
        private async Task UnMarkAsSeen(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = DController.UpdateData(await ApiService.SeenMovies.UnMarkAsSeenAsync(moviee, LoggedUser),
                LoggedUser,
                "UnMarkAsSeen",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateSeenList();
        }
        #endregion
        #region Like
        private async Task MarkAsLiked(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = await DController.UpdateData(await ApiService.LikedMovies.MarkAsLikedAsync(moviee, LoggedUser),
                LoggedUser,
                "MarkAsLiked",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateLikedList();
        }
        private async Task UnMarkAsLiked(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = DController.UpdateData(await ApiService.LikedMovies.UnMarkAsLikedAsync(moviee, LoggedUser),
                LoggedUser,
                "UnMarkAsLiked",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateLikedList();
        }
        #endregion
        #region Watch
        private async Task MarkAsWatch(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = await DController.UpdateData(await ApiService.WatchMovies.MarkAsWatchAsync(moviee, LoggedUser),
                LoggedUser,
                "MarkAsWatch",
                $"User {LoggedUser.Name} mark as seen movie {movie.Title}");
            UpdateWatchList();
        }
        private async Task UnMarkAsWatch(DetailedMovieModel movie)
        {
            var moviee = TransformMovieModel(movie);
            _ = DController.UpdateData(await ApiService.WatchMovies.UnMarkAsWatchAsync(moviee, LoggedUser),
                LoggedUser,
                "UnMarkAsWatch",
                $"User {LoggedUser.Name} unmark as seen movie {movie.Title}");

            UpdateWatchList();
        }
        #endregion

        private async Task UpdateSeenList()
        {
            SeenMovies = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            await InvokeAsync(StateHasChanged);
        }
        private async Task UpdateLikedList()
        {
            LikedMovies = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            await InvokeAsync(StateHasChanged);
        }
        private async Task UpdateWatchList()
        {
            WatchMovies = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            await InvokeAsync(StateHasChanged);
        }

        private MovieModel TransformMovieModel(DetailedMovieModel movie)
        {
            return new MovieModel()
            {
                Id = movie.Id,
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle, 
                Overview = movie.Overview, 
                PosterPath = movie.PosterPath, 
                BackdropPath = movie.BackdropPath, 
                ReleaseDate = movie.ReleaseDate, 
                VoteAverage = movie.VoteAverage, 
                VoteCount = movie.VoteCount, 
                Adult = movie.Adult, 
                OriginalLanguage = movie.OriginalLanguage, 
                Popularity = movie.Popularity
            };
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
