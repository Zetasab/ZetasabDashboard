using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Common.ZNT.Models;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZNT.NotesPageGrid
{
    public partial class NotesPageGrid
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
        private List<NoteModel> DataList = new();

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        //models
        private NoteModel InsertModel = new NoteModel();
        private NoteModel UpdateModel = new NoteModel();

        //loadings
        private bool datagridLoading = false;
        private bool insertDataLoading = false;
        private bool updateDataLoading = false;
        private bool deleteDataLoading = false;
        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPageAdmin);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en {ApiService.Notes._datos}",
                "Entrando en notas",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CService.UpdateCrumbItems("🛠️ Noteos", "/proyects", 2);
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
            InsertModel = new NoteModel();
            InsertModal = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(NoteModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            UpdateModal = true;
            StateHasChanged();
        }

        private async void OnOpenDeleteModal(NoteModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar {ApiService.Notes._ellaDato} {model.Title}?" }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<ConfirmDeleteDialog>("¡ATENCIÓN!", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await OnDeleteData();
                Console.WriteLine("si");
            }
        }
        #endregion

        #region CRUD
        #region Post
        private async Task OnInsertData()
        {
            insertDataLoading = true;
            await InvokeAsync(StateHasChanged);

            InsertModel.UpdatedAt = DateTime.Now;

            var result = await DController.InsertData(
                await ApiService.Notes.InsertNoteAsync(InsertModel, LoggedUser),
                LoggedUser,
                $"NotesPage:{nameof(OnInsertData)}",
                $"Insertando {ApiService.Notes._ellaDato} {InsertModel.Title}"
                );

            insertDataLoading = false;
            await InvokeAsync(StateHasChanged);

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
            datagridLoading = true;
            await InvokeAsync(StateHasChanged);

            DataList = await DController.GetData(await ApiService.Notes.GetAllNotesAsync(LoggedUser)) ?? new List<NoteModel>();

            datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Update
        private async Task OnUpdateData()
        {
            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            UpdateModel.UpdatedAt = DateTime.Now;

            var result = await DController.UpdateData(
                await ApiService.Notes.UpdateNoteAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"NotePage: {nameof(OnUpdateData)}",
                $"ActualizandoData {UpdateModel.Title}");

            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                UpdateModel = new NoteModel();
                UpdateModal = false;
                GetList();
            }
        }
        #endregion
        #region Delete
        private async Task OnDeleteData()
        {
            var result = await DController.DeleteData(
                await ApiService.Notes.DeleteNoteAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"NotePage: {nameof(OnDeleteData)}",
                $"Borrando {UpdateModel.Title}"
                );
            if (result)
            {
                UpdateModel = new NoteModel();
                GetList();
            }
        }
        #endregion
        #endregion

        #region Datagrid
        private MudDataGrid<NoteModel> _datagrid { get; set; }
        private string _searchString { get; set; }
        private Func<NoteModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.Content) && x.Content.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.CreatedAt.Value.ToLocalTime().ToString()) && x.CreatedAt.Value.ToLocalTime().ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.UpdatedAt.Value.ToLocalTime().ToString()) && x.UpdatedAt.Value.ToLocalTime().ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        #endregion
    }
}
