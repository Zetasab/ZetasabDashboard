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
using static MongoDB.Driver.WriteConcern;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV.HomeMoviePage
{
    public partial class HomeMoviePage
    {
        #region Vars
        #region Injects
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private CommonServices CService { get; set; } = default!;


        #endregion

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

        private string _searchMovie = "";
        private string _trendingMovie = "Hoy";

        private string _bg = "";
        public List<MovieModel> TrendingMovies { get; set; } = new List<MovieModel>();
        public List<MovieModel> CarteleraMovies { get; set; } = new List<MovieModel>();
        public List<MovieModel> PopularMovies { get; set; } = new List<MovieModel>();
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
                $"Entrando en Home movies",
                $"Entrando en Home movies",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetLists();
        }
        #endregion

        private async Task GetLists()
        {
            TrendingMovies = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTrendingAsync("day",LoggedUser));
            CarteleraMovies = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingAsync(1,LoggedUser));
            PopularMovies = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularAsync(1,LoggedUser));


            var rand = new Random();

            _bg = TrendingMovies[rand.Next(0, 20)].BackdropPath;

            await InvokeAsync(StateHasChanged);
        }
        private async Task NavigateToSearchMovie()
        {
            Navigator.NavigateTo($"search-movie/?mode=search&search={_searchMovie}");
        }

        private async Task ChangeTrendingMovies(string text)
        {
            if (text == "Hoy")
            {
                _trendingMovie = text;
                TrendingMovies = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTrendingAsync("day", LoggedUser));
            }
            else 
            {
                _trendingMovie = "Esta semana";
                TrendingMovies = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTrendingAsync("week", LoggedUser));
            }
        }
    }
}
