using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor;
using System.Collections;
using System.Runtime.CompilerServices;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using ZetaDashboard.Shared.ConfirmDeleteDialog;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.ZDB.CollectionsPage
{
    public partial class CollectionsPage
    {
        #region Injects
        [Inject] MongoInfoService MongoService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] IJSRuntime JS { get; set; }
        [Inject] BaseService ApiService { get; set; }


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
        private List<MongoCollectionModel> CollectionNames = new();
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
                "Collections",
                "Entrando en collections",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            GetCollectionList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CService.UpdateCrumbItems("📉 Colecciones", "/collections",2);
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
        private async Task GetCollectionList()
        {
            CollectionNames = await MongoService.GetCollectionInfoAsync();
            UpdateDataCharts();
            await InvokeAsync(StateHasChanged);
        }

        #endregion
        #region Delete
        private async Task DeleteCollection(string name)
        {
            
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar la coleccion {name}?" }
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<ConfirmDeleteDialog>("¡ATENCIÓN!", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                if(await MongoService.DeleteCollection(name))
                {
                    Snackbar.Add($"Coleccion {name} borrada correctamente", Severity.Success);
                    GetCollectionList();
                }
                else
                {
                    Snackbar.Add($"Error borrando {name}", Severity.Error);
                }
            }
        }
        #endregion
        #endregion

        #region Event
        private async Task BackUpCollection(string name)
        {
            string json = await MongoService.GetBackUpCollectionAsync(name);
            await JS.InvokeVoidAsync("downloadJsonFile", $"{name}.json", json);
        }
        #endregion

        #region Charts
        public double[] _donutData = { 25, 77, 28, 5 };
        public string[] _donut_labels = { "Oil", "Coal", "Gas", "Biomass" };

        public double[] _pieData = { 25, 77, 28, 5 };
        public string[] _pie_labels = { "Oil", "Coal", "Gas", "Biomass" };
        private string[] _colors = {"", "", "", "", "", "", "", "", "", "", "", "", "", "", ""};
        private void UpdateDataCharts()
        {
            var rnd = new Random();
            _colors = Enumerable.Range(0, 20)
                .Select(_ => $"rgb({rnd.Next(256)}, {rnd.Next(256)}, {rnd.Next(256)})")
                .ToArray();

            _donutData = CollectionNames.Select(x => x.Count).ToArray();
            _donut_labels = CollectionNames.Select(x => x.Name).ToArray();

            _pieData = CollectionNames.Select(x => x.Size).ToArray();
            _pie_labels = CollectionNames.Select(x => x.Name).ToArray();
            StateHasChanged();
        }
        #endregion
    }
}
