using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.PLN.Models;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.PLN.PlansListPage
{
    public partial class PlansListPage
    {

        #region Injects
        [Inject] BaseService ApiService { get; set; }
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
            Code = "znt",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "znt",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "znt",
            UserType = EUserPermissionType.Admin
        };
        #endregion
        private List<PlanListModel> DataList = new();

        
        //loadings
        private bool datagridLoading = false;
        private bool datagrid2Loading = false;
        private bool insertDataLoading = false;
        private bool updateDataLoading = false;
        private bool deleteDataLoading = false;
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
                $"Entrando en {ApiService.Plans._datos}",
                $"Entrando en {ApiService.Plans._datos}",
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
            datagridLoading = true;
            await InvokeAsync(StateHasChanged);

            DataList = await DController.GetData(await ApiService.Plans.GetAllPlanListsByUserAsync(LoggedUser)) ?? new List<PlanListModel>();

            datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

        private async Task NavigateTo(string id)
        {
            Navigator.NavigateTo($"/planlist/{id}");
        }
    }
}
