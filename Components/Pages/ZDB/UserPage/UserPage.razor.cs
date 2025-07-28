using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Driver;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZDB.UserPage
{
    public partial class UserPage
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

        private List<UserModel> DataList = new();
        private List<ProyectModel> DataProyectList = new();

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        //models
        private UserModel InsertModel = new UserModel();
        private UserModel UpdateModel = new UserModel();
        string updatePassword = "";
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
                "Users",
                "Entrando en usuarios",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
            GetProyectoList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CService.UpdateCrumbItems("👨‍👩‍👦‍👦 Usuarios", "/user", 2);
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

        #region Events
        private async void OnOpenInsertModal()
        {
            _icon = Icons.Material.Filled.VisibilityOff;
            _inputType = InputType.Password;
            InsertModel = new UserModel();
            InsertModal = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(UserModel model)
        {
            updatePassword = "";
            _icon = Icons.Material.Filled.VisibilityOff;
            _inputType = InputType.Password;
            UpdateModel = await DController.DeepCoopy(model);
            foreach(var item in DataProyectList)
            {
                if(UpdateModel.Permissions.FirstOrDefault(x => x.Code == item.Code) == null)
                {
                    UpdateModel.Permissions.Add(new UserPermissions() { Code = item.Code, UserType = EUserPermissionType.None });
                }
            }
            UpdateModal = true;
            StateHasChanged();
        }

        private async void OnOpenDeleteModal(UserModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el usuario {model.Name}?" }
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

        private InputType _inputType { get; set; } = InputType.Password;
        private string _icon { get; set; } = Icons.Material.Filled.VisibilityOff;

        private void _changeVisibility()
        {
            if(_inputType == InputType.Password)
            {
                _inputType = InputType.Text;
                _icon = Icons.Material.Filled.Visibility;
            }
            else
            {
                _inputType = InputType.Password;
                _icon = Icons.Material.Filled.VisibilityOff;
            }
        }

        private void PermissionsValueChanged(string code,EUserPermissionType type)
        {
            var permission = new UserPermissions()
            {
                Code = code,
                UserType = type
            };
            var x = InsertModel.Permissions.FirstOrDefault(x => x.Code == code);
            if (x != null)
            {
                x.UserType = permission.UserType;
            }
            else
            {
                InsertModel.Permissions.Add(permission);
            }
        }
        private void PermissionsUpdateValueChanged(string code, EUserPermissionType type)
        {
            var permission = new UserPermissions()
            {
                Code = code,
                UserType = type
            };
            var x = UpdateModel.Permissions.FirstOrDefault(x => x.Code == code);
            if (x != null)
            {
                x.UserType = permission.UserType;
            }
            else
            {
                UpdateModel.Permissions.Add(permission);
            }
        }

        #endregion

        #region CRUD
        #region Post
        private async Task OnInsertData()
        {
            InsertModel.PasswordHash = BCrypt.Net.BCrypt.HashPassword(InsertModel.PasswordHash);
            var result = await DController.InsertData(
                await ApiService.Users.InsertUserAsync(InsertModel, LoggedUser),
                LoggedUser,
                $"UserPage: {nameof(OnInsertData)}",
                $"Insertando usuario {InsertModel.Name}");
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
            DataList = await DController.GetData(await ApiService.Users.GetAllUsersAsync(LoggedUser)) ?? new List<UserModel>();
            await InvokeAsync(StateHasChanged);
        }
        private async Task GetProyectoList()
        {
            DataProyectList = await DController.GetData(await ApiService.Proyects.GetAllProyectsAsync(LoggedUser)) ?? new List<ProyectModel>();

            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Update
        private async Task OnUpdateData()
        {
            if(updatePassword != "")
            {
                UpdateModel.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatePassword);
            }

            var result = await DController.UpdateData(
                await ApiService.Users.UpdateUserAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"UserPage: {nameof(OnUpdateData)}",
                $"Actualizando {UpdateModel.Name}"
                );
            if (result)
            {
                UpdateModel = new UserModel();
                UpdateModal = false;
                GetList();
            }
        }
        #endregion
        #region Delete
        private async Task OnDeleteData()
        {
            var result = await DController.DeleteData(
                await ApiService.Users.DeleteUserAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"UserPage: {nameof(OnDeleteData)}",
                $"Borrando {UpdateModel.Name}");
            if (result)
            {
                UpdateModel = new UserModel();
                GetList();
            }
        }
        #endregion
        #endregion

        #region Datagrid
        private MudDataGrid<UserModel> _datagrid { get; set; }
        private string _searchString { get; set; }
        private Func<UserModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion
    }
}
