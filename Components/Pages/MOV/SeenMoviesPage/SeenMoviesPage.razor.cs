using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static MongoDB.Driver.WriteConcern;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV.SeenMoviesPage
{
    public partial class SeenMoviesPage
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
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
        private List<MovieModel> DataList { get; set; } = new List<MovieModel>();
        private List<MovieModel> SeenMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> LikedMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> WatchMovieList { get; set; } = new List<MovieModel>();

        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en SeenMovies",
                $"Entrando en SeenMovies",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }   

        private async Task GetList()
        {
            DataList = await DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser));

            SeenMovieList = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            LikedMovieList = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            WatchMovieList = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();

           await InvokeAsync(StateHasChanged);
            
        }
    }
}
