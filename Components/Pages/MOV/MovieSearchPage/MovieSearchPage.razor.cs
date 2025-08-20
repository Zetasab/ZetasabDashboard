using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV.MovieSearchPage
{
    public partial class MovieSearchPage
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

        private List<MovieModel> DataList { get; set; } = new List<MovieModel>();

        private int _pag = 1;
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
                $"Entrando en SeachMovie",
                $"Entrando en SeachMovie",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        #endregion
        #region CRUD
        #region GET
        private async Task GetList()
        {
            //datagridLoading = true;
            //await InvokeAsync(StateHasChanged);
            _pag = 1;
            
            DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser)));

            //datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

        private Color GetScoreColor(double voteAverage)
        {
            var percent = voteAverage * 10; // 0-100
            if (percent >= 70) return Color.Success;   // verde
            if (percent >= 40) return Color.Warning;   // amarillo
            return Color.Error;                        // rojo
        }

        private async Task LoadMore()
        {
            _pag++;
            DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser)));
        }

    }
}
