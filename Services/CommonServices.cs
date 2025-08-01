using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;
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
        public void CheckSuperAdminPermissions(UserModel user)
        {
            if (user.UserType < EUserType.SuperAdmin) 
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
        #region BreadCrumItems
        public event Action? OnBreadcrumbChanged;
        public List<BreadcrumbItem> BreadcrumbItems { get; set; } = new List<BreadcrumbItem>();
        /// <summary>
        /// Update CrumItems depeding of what sublevel it's
        /// </summary>
        /// <param name="name">displayed name</param>
        /// <param name="path">path name</param>
        /// <param name="level">what level is</param>
        /// <param name="disabled">if is disabled or not</param>
        public async Task UpdateCrumbItems(string name, string path, int level = 1, bool disabled = false)
        {
            if (level == 1)
            {
                //Level Home
                BreadcrumbItems.Clear();
                BreadcrumbItems.Add(new BreadcrumbItem("🏡Inicio", "/home", false));

            }
            else if (level > BreadcrumbItems.Count)
            {
                //level from home
                BreadcrumbItems.Add(new BreadcrumbItem(name, path, disabled));
            }
            else if (level == BreadcrumbItems.Count)
            {
                //same level
                BreadcrumbItems.RemoveAt(BreadcrumbItems.Count - 1);
                BreadcrumbItems.Add(new BreadcrumbItem(name, path, disabled));
            }
            else
            {
                //previus level
                var actualsBreadcrumitems = BreadcrumbItems;
                int deepLeves = BreadcrumbItems.Count - level;

                int indexStart = Math.Max(0, BreadcrumbItems.Count - deepLeves);

                BreadcrumbItems.RemoveRange(indexStart, deepLeves);
                BreadcrumbItems.Add(new BreadcrumbItem(name, path, disabled));
            }
            OnBreadcrumbChanged?.Invoke();
        }

        #endregion

        public MarkupString FormatNoteContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new MarkupString("");

            // Detecta https:// y http://
            string pattern = @"(https?:\/\/[^\s]+)";
            string processed = Regex.Replace(content, pattern, "<a href=\"$1\" target=\"_blank\" style=\"color:#4EA1D3;text-decoration:underline;\">$1</a>");

            // Reemplaza \n por <br>
            processed = processed.Replace("\n", "<br>");

            return new MarkupString(processed);
        }
    }
}
