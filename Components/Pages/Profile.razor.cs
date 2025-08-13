using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ZetaCommon.Auth;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages
{
    public partial class Profile
    {
        #region Injects
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] BaseService ApiService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] NavigationManager Navigator { get; set; }
        [Inject] DataController DController { get; set; }


        #endregion

        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }
        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "zdb",
            UserType = EUserPermissionType.Visor
        };
        #endregion
        #endregion
        #region LifeCycles
        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                "Users",
                $"Entrando en perfil de {LoggedUser.Name}",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
        }
        #endregion
        
        private async Task UpdateTheme(string theme)
        {
            LoggedUser.MudCustomTheme = theme;
            var result = await DController.UpdateData(await ApiService.Users.UpdateProfileAsync(LoggedUser, LoggedUser), LoggedUser,
                "Profile update user theme",
                $"Updated user {LoggedUser.Name} theme from profile");
            if (result)
            {
                (AuthProvider as CustomAuthenticationStateProvider).UpdateUser(LoggedUser);
                Navigator.NavigateTo("/profile", true);
            }
        }
    }
}
