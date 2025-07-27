using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Services
{
    public class CommonServices
    {
        #region Injects
        public NavigationManager  Navigator { get; set; }
        public ISnackbar  Snackbar { get; set; }

        public CommonServices(NavigationManager navigator, ISnackbar snackbar) 
        {
            Navigator = navigator;
            Snackbar = snackbar;
        }
        #endregion

        #region Permissions
        public bool CheckLoginPermissions(UserModel user)
        {
            if(user.UserType == EUserType.SuperAdmin)
            {
                return true;
            }
            foreach (var perm in user.Permissions)
            {
                if(perm.UserType != EUserPermissionType.None)
                {
                    return true;
                }
            }
                return false;
        }
        public bool CheckIfPermissions(UserModel user,UserPermissions permisions)
        {
            if(user.UserType == EUserType.SuperAdmin)
            {
                return true;
            }
            foreach (var perm in user.Permissions)
            {
                if (perm.Code == permisions.Code && perm.UserType >= permisions.UserType)
                {
                    return true;
                }
            }
            return false;
        }
        public void CheckPermissions(UserModel user, UserPermissions permisions)
        {
            if(!CheckIfPermissions(user, permisions))
            {
                Snackbar.Add("No tienes los suficientes permisos", Severity.Warning);
                Navigator.NavigateTo("/");
            }
        }
        #endregion
        #region Icons
        public string EUserTypeIcon(EUserType user) => user switch
        {
            EUserType.Restricted => Icons.Material.Filled.PersonOff,
            EUserType.SuperAdmin => Icons.Material.Filled.Security
        };

        public string EUserPermissionTypeIcon(EUserPermissionType user) => user switch
        {
            EUserPermissionType.None => Icons.Material.Filled.Close,
            EUserPermissionType.Visor => Icons.Material.Filled.Visibility,
            EUserPermissionType.Editor => Icons.Material.Filled.Edit,
            EUserPermissionType.Admin => Icons.Material.Filled.AdminPanelSettings
        };
        #endregion

        #region Color
        public enum ColorType
        {
            Normal,
            Darken,
            Lighten
        }
        public string GetCssVar(Color color, ColorType colorType = ColorType.Normal)
        {
            var colorToReturn = color switch
            {
                Color.Primary => "var(--mud-palette-primary",
                Color.Secondary => "var(--mud-palette-secondary",
                Color.Tertiary => "var(--mud-palette-tertiary",
                Color.Info => "var(--mud-palette-info",
                Color.Success => "var(--mud-palette-success",
                Color.Warning => "var(--mud-palette-warning",
                Color.Error => "var(--mud-palette-error",
                Color.Dark => "var(--mud-palette-dark",
                _ => "inherit"
            };
            colorToReturn = colorToReturn + colorType switch
            {
                ColorType.Normal => ")",
                ColorType.Darken => "-darken)",
                ColorType.Lighten => "-lighten)",
                _ => ")"
            };

            return colorToReturn;
        }
        #endregion
    }
}
