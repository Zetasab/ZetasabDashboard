using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;

namespace ZetaDashboard.Components.Pages.ZDB.ProyectsPage
{
    public partial class ProyectsPage
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] DataController DController { get; set; }
        #endregion

        #region Vars
        private List<ProyectModel> DataList = new();

        //modals
        private bool InsertModal { get; set; } = false;
        //models
        private ProyectModel InsertModel = new ProyectModel();
        #endregion

        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            GetList();
        }
        #endregion

        #region Events
        private async void OnOpenInsertModal()
        {
            InsertModel = new ProyectModel();
            InsertModal = true;
        }
        #endregion

        #region CRUD
        #region Insert
        private async Task OnInsertData()
        {
            InsertModel.FullName = $"{InsertModel.Code}-{InsertModel.Name}";
            InsertModel.Name = InsertModel.Name.ToLower();
            InsertModel.Code = InsertModel.Code.ToLower();
            InsertModel.Url = InsertModel.Url?.ToLower();
            var result = await DController.InsertData(await ApiService.Proyects.InsertProyectAsync(InsertModel));
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
            DataList = await DController.GetData(await ApiService.Proyects.GetAllProyectsAsync()) ?? new List<ProyectModel>();
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #region Delete
        //private async Task DeleteCollection(string name)
        //{
        //    if (await MongoService.DeleteCollection(name))
        //    {
        //        Snackbar.Add($"Coleccion {name} borrada correctamente", Severity.Success);
        //    }
        //    else
        //    {
        //        Snackbar.Add($"Error borrando {name}", Severity.Error);
        //    }
        //    GetCollectionList();
        //}
        #endregion
        #endregion
    }
}
