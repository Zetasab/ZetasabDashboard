using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor;
using System.Collections;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
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
            GetCollectionList();
        }
        #endregion

        #region CRUD
        #region Get
        private async Task GetCollectionList()
        {
            CollectionNames = await MongoService.GetCollectionInfoAsync();
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
    }
}
