using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.PLN.Models;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.PLN.PlansListPage
{
    public partial class PlansPage
    {
        [Parameter]
        public string planlistid { get; set; }

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

        private PlanListModel PlansListPage { get; set; } = new PlanListModel();
        private string DefaultImg => "https://images.unsplash.com/photo-1520975922323-3d8c0d9d4c54?q=80&w=800&auto=format&fit=crop";

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        private bool MarkAsSeenModal { get; set; } = false;
        //models
        private PlanModel InsertModel = new PlanModel();
        private PlanModel UpdateModel = new PlanModel();

        //loaadings
        private bool insertDataLoading = false;
        private bool updateDataLoading = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en {ApiService.Plans._datos} de {planlistid}",
                $"Entrando en {ApiService.Plans._datos} de {planlistid}",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetList();
        }

        #region CRUD
        #region GET
        private async Task GetList()
        {
            //datagridLoading = true;
            await InvokeAsync(StateHasChanged);

            PlansListPage = await DController.GetData(await ApiService.Plans.GetPlanListByIdAsync(planlistid,LoggedUser)) ?? new PlanListModel();

            //datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #region Post
        private async Task OnInsertData()
        {
            insertDataLoading = true;
            await InvokeAsync(StateHasChanged);

            PlansListPage.Plans.Add(InsertModel);

            var result = await DController.UpdateData(
               await ApiService.Plans.UpdatePlanListAsync(PlansListPage, LoggedUser),
               LoggedUser,
               $"PlanPage: {PlansListPage.Name} {nameof(OnInsertData)}",
               $"ActualizandoData {UpdateModel.Name}");

            insertDataLoading = false;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                InsertModal = false;
                GetList();
            }
        }
        #endregion
        #region Update
        private async Task OnUpdateData()
        {
            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            if(UpdateModel.Score != -1)
            {
                UpdateModel.Status = PlanStatus.Done;
            }
            else
            {
                UpdateModel.Status = PlanStatus.Pending;
            }

                var index = PlansListPage.Plans.FindIndex(p => p.Id == UpdateModel.Id);
            if (index != -1)
            {
                PlansListPage.Plans[index] = UpdateModel;
            }

            var result = await DController.UpdateData(
                await ApiService.Plans.UpdatePlanListAsync(PlansListPage, LoggedUser),
                LoggedUser,
                $"PlanPage: {PlansListPage.Name} {nameof(OnUpdateData)}",
                $"ActualizandoData {UpdateModel.Name}");

            updateDataLoading = true;
            await InvokeAsync(StateHasChanged);

            if (result)
            {
                UpdateModel = new PlanModel();
                UpdateModal = false;
                MarkAsSeenModal = false;
                GetList();
            }
        }
        #endregion
        #region Delete
        private async void OnOpenDeleteModal(PlanModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
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
        private async Task OnDeleteDataPlan()
        {

            PlansListPage.Plans.RemoveAll(x => x.Id == UpdateModel.Id);

            var result = await DController.UpdateData(
              await ApiService.Plans.UpdatePlanListAsync(PlansListPage, LoggedUser),
              LoggedUser,
              $"PlanPage: {PlansListPage.Name} {nameof(OnUpdateData)}",
                $"ActualizandoBorrandoData {UpdateModel.Name}");
            if (result)
            {
                UpdateModel = new PlanModel();
                GetList();
            }
        }
        #endregion
        #endregion
        #region Events
        private async void OnOpenInsertModal()
        {
            InsertModel = new PlanModel();
            InsertModal = true;
            StateHasChanged();
        }
        private async void OnOpenUpdateModal(PlanModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            UpdateModal = true;
            StateHasChanged();
        }
        private async void OnMarkAsSeenModal(PlanModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            MarkAsSeenModal = true;
            StateHasChanged();
        }
        private async void OnUnMarkAsSeenModal(PlanModel model)
        {
            UpdateModel = await DController.DeepCoopy(model);
            UpdateModel.Score = -1;
            OnUpdateData();
            StateHasChanged();
        }
        
        #endregion

        public string GetModeIconn(PlanMode plan)
        {
            return plan switch
            {
                PlanMode.Chill => Icons.Material.Filled.SelfImprovement,
                PlanMode.Casa => Icons.Material.Filled.Home,
                PlanMode.Salir => Icons.Material.Filled.Nightlife,
                PlanMode.Planificar => Icons.Material.Filled.EditCalendar,
                _ => Icons.Material.Filled.HelpOutline
            };
        }

        public Color GetModeColorr(PlanMode plan) => plan switch
        {
            PlanMode.Chill => Color.Info,     // azul relajado
            PlanMode.Casa => Color.Success,  // verde "home"
            PlanMode.Salir => Color.Warning,  // ámbar enérgico
            PlanMode.Planificar => Color.Primary,  // color de marca/acción
            _ => Color.Default
        };

    }
}
