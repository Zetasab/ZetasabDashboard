using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZDB.AuditsPage
{
    public partial class AuditsPage
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        #endregion

        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }
        
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "zdb",
            UserType = EUserPermissionType.Admin
        };
        #endregion
        private List<AuditModel> DataList = new();

        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckSuperAdminPermissions(LoggedUser);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                "Audits",
                "Entrando en audits",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            await GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CService.UpdateCrumbItems("📰 Auditoria", "/audits",2);
                await Task.Delay(100);
                CService.OnBreadcrumbChanged += OnBreadcrumbsChanged;
            }
        }
        private void OnBreadcrumbsChanged()
        {
            InvokeAsync(StateHasChanged); // forzar que el layout se actualice
        }
        public void Dispose()
        {
            CService.OnBreadcrumbChanged -= OnBreadcrumbsChanged;
        }
        #endregion
        #region CRUD
        #region Get
        private async Task GetList()
        {
            DataList = await DController.GetData(await ApiService.Audits.GetAllAuditsAsync(LoggedUser)) ?? new List<AuditModel>();
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

        #region Datagrid
        private MudDataGrid<AuditModel> _datagrid { get; set; }
        private string _searchString { get; set; }
        private Func<AuditModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrEmpty(x.Id) && x.Id.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.UserId) && x.UserId.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.UserName) && x.UserName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.What.ToString()) && x.What.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.Where) && x.Where.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(x.When.ToString("dd/MM/yyyy HH:mm")) && x.When.ToString("dd/MM/yyyy HH:mm").Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            return false;
        };

        #endregion


    }
}
