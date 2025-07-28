using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages
{
    public partial class Home
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;

        #endregion

        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }

        #endregion
        private List<ProyectModel> DataList = new();
        #endregion
        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CService.UpdateCrumbItems("🏡 Inicio", "/");
                await InvokeAsync(StateHasChanged);
            } 
        }
        #endregion

        #region Crud
        #region Get
        private async Task GetList()
        {
            DataList = await DController.GetData(await ApiService.Proyects.GetAllProyectsAsync(LoggedUser)) ?? new List<ProyectModel>();
            DataList.Insert(0, new ProyectModel() { Code = "rlw", Name = "Railway", Url = "https://railway.com/dashboard" });
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

        private void OnClick(string url)
        {
            Navigator.NavigateTo(url);
        }
    }
}
