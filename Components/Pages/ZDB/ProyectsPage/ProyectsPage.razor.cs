using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static MudBlazor.CategoryTypes;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZDB.ProyectsPage
{
    public partial class ProyectsPage
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
        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "zdb",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "zdb",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "zdb",
            UserType = EUserPermissionType.Admin
        };
        #endregion
        private List<ProyectModel> DataList = new();

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        //models
        private ProyectModel InsertModel = new ProyectModel();
        private ProyectModel UpdateModel = new ProyectModel();
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
                "Proyects",
                "Entrando en proyectos",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }
        #endregion

        #region Events
        private async void OnOpenInsertModal()
        {
            InsertModel = new ProyectModel();
            InsertModal = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(ProyectModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            UpdateModal = true;
            StateHasChanged();
        }

        private async void OnOpenDeleteModal(ProyectModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el proyecto {model.FullName}?" }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<ConfirmDeleteDialog>("¡ATENCIÓN!", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await OnDeleteData(); // ← tu método real
                Console.WriteLine("si");
            }
        }
        #endregion

        #region CRUD
        #region Post
        private async Task OnInsertData()
        {
            InsertModel.FullName = $"{InsertModel.Code}-{InsertModel.Name}";
            InsertModel.Name = InsertModel.Name.ToLower();
            InsertModel.Code = InsertModel.Code.ToLower();
            var result = await DController.InsertData(
                await ApiService.Proyects.InsertProyectAsync(InsertModel, LoggedUser),
                LoggedUser,
                $"Proyects:{nameof(OnInsertData)}",
                $"Insertando proyecto {InsertModel.FullName}"
                );
            if (result)
            {
                InsertModal = false;
                GetList();
            }
        }
        #endregion
        #region Get
        private async Task GetList()
        {
            DataList = await DController.GetData(await ApiService.Proyects.GetAllProyectsAsync(LoggedUser)) ?? new List<ProyectModel>();
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        
        #region Update
        private async Task OnUpdateData()
        {
            UpdateModel.FullName = $"{UpdateModel.Code}-{UpdateModel.Name}";
            UpdateModel.Name = UpdateModel.Name.ToLower();
            UpdateModel.Code = UpdateModel.Code.ToLower();
            var result = await DController.UpdateData(
                await ApiService.Proyects.UpdateProyectAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"ProyectPage: {nameof(OnUpdateData)}",
                $"ActualizandoData {UpdateModel.FullName}");
            if (result)
            {
                UpdateModel = new ProyectModel();
                UpdateModal = false;
                GetList();
            }
        }
        #endregion
        #region Delete
        private async Task OnDeleteData()
        {
            var result = await DController.DeleteData(
                await ApiService.Proyects.DeleteProyectAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"ProyectPage: {nameof(OnDeleteData)}",
                $"Borrando {UpdateModel.FullName}"
                );
            if (result)
            {
                UpdateModel = new ProyectModel();
                GetList();
            }
        }
        #endregion
        #endregion

        #region Datagrid
        private MudDataGrid<ProyectModel> _datagrid { get; set; }
        private string _searchString { get; set; }
        private Func<ProyectModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrEmpty(x.Code) && x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.FullName) && x.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.Url) && x.Url.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion
    }
}
