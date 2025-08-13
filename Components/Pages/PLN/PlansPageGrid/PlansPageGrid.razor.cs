using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.PLN.Models;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Common.ZNT.Models;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.PLN.PlansPageGrid
{
    public partial class PlansPageGrid
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
        private List<PlanListModel> DataList = new();

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        private bool InsertModalPlan { get; set; } = false;
        private bool UpdateModalPlan { get; set; } = false;
        //models
        private PlanListModel InsertModel = new PlanListModel();
        private PlanListModel UpdateModel = new PlanListModel();

        private PlanModel InsertModelPlan = new PlanModel();
        private PlanModel UpdateModelPlan = new PlanModel();

        private List<PlanModel> SelectedPlans = new List<PlanModel>();

        private List<UserModel> UsersToSelect = new List<UserModel>();
        private IEnumerable<string> UserIds { get; set; } = new HashSet<string>() { };
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
            CService.CheckPermissions(LoggedUser, ThisPageAdmin);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en {ApiService.Plans._datos}",
                "Entrando en notas",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetOtherLists();
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CService.UpdateCrumbItems("🛠️ PlanListos", "/proyects", 2);
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
            InsertModel = new PlanListModel();
            UserIds = new HashSet<string>();
            InsertModal = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(PlanListModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            UserIds = UpdateModel.UsersIds;
            UpdateModal = true;
            StateHasChanged();
        }
        private async void OnSeePlans(PlanListModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            SelectedPlans = UpdateModel.Plans;
            StateHasChanged();
        }
        private async void OnOpenInsertModalPlan()
        {
            InsertModelPlan = new PlanModel();
            InsertModalPlan = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModalPlan(PlanModel model)
        {
            UpdateModelPlan = await DController.DeepCoopy(model);
            UpdateModalPlan = true;
            StateHasChanged();
        }

        private async void OnOpenDeleteModal(PlanListModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar {ApiService.Plans._ellaDato} {model.Name}?" }
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
        private async void OnOpenDeleteModalPlan(PlanModel model)
        {
            UpdateModelPlan = await DController.DeepCoopy(model);
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el plan {model.Name}?" }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<ConfirmDeleteDialog>("¡ATENCIÓN!", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                await OnDeleteDataPlan();
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

            InsertModel.UsersIds = UserIds.ToList();

            var result = await DController.InsertData(
                await ApiService.Plans.InsertPlanListAsync(InsertModel, LoggedUser),
                LoggedUser,
                $"PlanListsPage:{nameof(OnInsertData)}",
                $"Insertando {ApiService.Plans._ellaDato} {InsertModel.Name}"
                );

            insertDataLoading = false;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                InsertModal = false;
                GetList();
            }
        }
        private async Task OnInsertDataPlan()
        {
            insertDataLoading = true;
            await InvokeAsync(StateHasChanged);

            UpdateModel.Plans.Add(InsertModelPlan);

            var result = await DController.UpdateData(
               await ApiService.Plans.UpdatePlanListAsync(UpdateModel, LoggedUser),
               LoggedUser,
               $"PlanListPage: {nameof(OnUpdateData)}",
               $"ActualizandoData {UpdateModel.Name}");

            insertDataLoading = false;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                InsertModalPlan = false;
                GetList();
            }
        }
        #endregion
        #region Get
        private async Task GetList()
        {
            datagridLoading = true;
            await InvokeAsync(StateHasChanged);

            DataList = await DController.GetData(await ApiService.Plans.GetAllPlanListsAsync(LoggedUser)) ?? new List<PlanListModel>();

            datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        private async Task GetOtherLists()
        {
            UsersToSelect = await DController.GetData(await ApiService.Users.GetAllUsersAsync(LoggedUser)) ?? new List<UserModel>();
        }
        
        #endregion

        #region Update
        private async Task OnUpdateData()
        {
            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            UpdateModel.UsersIds = UserIds.ToList();

            var result = await DController.UpdateData(
                await ApiService.Plans.UpdatePlanListAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"PlanListPage: {nameof(OnUpdateData)}",
                $"ActualizandoData {UpdateModel.Name}");

            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                UpdateModel = new PlanListModel();
                UpdateModal = false;
                GetList();
            }
        }
        private async Task OnUpdateDataPlan()
        {
            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            var index = UpdateModel.Plans.FindIndex(p => p.Id == UpdateModelPlan.Id);
            if (index != -1)
            {
                UpdateModel.Plans[index] = UpdateModelPlan;
            }

            var result = await DController.UpdateData(
                await ApiService.Plans.UpdatePlanListAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"PlanListPage: {nameof(OnUpdateData)}",
                $"ActualizandoData {UpdateModel.Name}");

            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                UpdateModelPlan = new PlanModel();
                UpdateModalPlan = false;
                GetList();
            }
        }
        #endregion
        #region Delete
        private async Task OnDeleteData()
        {
            var result = await DController.DeleteData(
                await ApiService.Plans.DeletePlanListAsync(UpdateModel, LoggedUser),
                LoggedUser,
                $"PlanListPage: {nameof(OnDeleteData)}",
                $"Borrando {UpdateModel.Name}"
                );
            if (result)
            {
                UpdateModel = new PlanListModel();
                GetList();
            }
        }
        private async Task OnDeleteDataPlan()
        {

            UpdateModel.Plans.RemoveAll(x => x.Id == UpdateModelPlan.Id);

            var result = await DController.UpdateData(
              await ApiService.Plans.UpdatePlanListAsync(UpdateModel, LoggedUser),
              LoggedUser,
              $"PlanListPage: {nameof(OnUpdateData)}",
              $"ActualizandoData {UpdateModel.Name}");
            if (result)
            {
                UpdateModelPlan = new PlanModel();
                GetList();
            }
        }
        #endregion
        #endregion

        #region Datagrid
        private MudDataGrid<PlanListModel> _datagrid { get; set; }
        private string _searchString { get; set; }
        private Func<PlanListModel, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            return false;
        };

        private string _searchString2 { get; set; }
        private Func<PlanModel, bool> _quickFilter2 => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString2))
                return true;

            if (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            return false;
        };

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            var selectedusers = new List<string>();
            foreach (var item in selectedValues)
            {
                selectedusers.Add(UsersToSelect.FirstOrDefault(x => x.Id == item).Name ?? item);
            }
            return $"Seleccionado{(selectedValues.Count > 1 ? "s" : "")}: {string.Join(", ", selectedusers)}";
        }
        #endregion
    }
}
