using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Pages.ZDB.CollectionsPage
{
    public partial class CollectionsPage
    {
        #region Injects
        [Inject] MongoInfoService MongoService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        #endregion

        #region Vars
        private List<MongoCollectionModel> CollectionNames = new();
        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
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
            if(await MongoService.DeleteCollection(name))
            {
                Snackbar.Add($"Coleccion {name} borrada correctamente",Severity.Success);
            }
            else
            {
                Snackbar.Add($"Error borrando {name}",Severity.Error);
            }
            GetCollectionList();
        }
        #endregion
        #endregion
    }
}
