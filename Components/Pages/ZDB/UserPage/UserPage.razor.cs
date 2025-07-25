using Microsoft.AspNetCore.Components;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;

namespace ZetaDashboard.Components.Pages.ZDB.UserPage
{
    public partial class UserPage
    {
        #region Vars

        #region Injects
        [Inject] BaseService ApiService { get; set; }

        #endregion

        //models
        private List<User> UserList { get; set; } = new List<User>();
        private User InsertUser = new User();

        //modals
        private bool InsertModal { get; set; } = false;
        private bool UpdateModal { get; set; } = false;
        private bool DeleteModal { get; set; } = false;

        #endregion


        #region LifeCycles

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UpdateList();

                
            }
        }
        #endregion

        #region Crud

        #region Get
        private async Task UpdateList()
        {
            UserList = await ApiService.GetAllUsersAsync();
            Console.WriteLine("db:");
            foreach (var item in UserList)
            {
                Console.WriteLine(item);
            }
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Insert
        private async void InsertOverlay()
        {
            InsertUser = new User();
            InsertModal = true;
        }
        private async Task OnInsertData()
        {
            //_ = DController.InsertData(await Db.InsertEvents(InsertEvent));
            await ApiService.AddUserAsync(InsertUser);
            UpdateList();
            InsertModal = false;
        }
        #endregion

        #endregion
    }
}
