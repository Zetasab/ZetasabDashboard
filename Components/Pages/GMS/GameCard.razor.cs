using Microsoft.AspNetCore.Components;
using MudBlazor;
using ZetaCommon.Auth;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.GMS
{
    public partial class GameCard
    {
        [Parameter] public RawgGame item { get; set; }
        [Parameter] public List<RawgGame> LikedGames { get; set; }
        [Parameter] public List<RawgGame> PlayedGames { get; set; }
        [Parameter] public List<RawgGame> WatchedGames { get; set; }
        [Parameter] public BaseService ApiService { get; set; }
        [Parameter] public DataController DController { get; set; }
        [Parameter] public UserModel LoggedUser { get; set; }
        [Parameter] public CommonServices CService { get; set; }
        [Parameter] public NavigationManager Navigator { get; set; }
        [Parameter] public EventCallback Update { get; set; }
        [Parameter] public bool ShowButtons { get; set; } = true;
        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "gms",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "gms",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "gms",
            UserType = EUserPermissionType.Admin
        };

        private Color GetScoreColor(double voteAverage)
        {
            var percent = voteAverage * 20; // 0-100
            if (percent >= 70) return Color.Success;   // verde
            if (percent >= 40) return Color.Warning;   // amarillo
            return Color.Error;                        // rojo
        }

        #region liked
        private async Task MarkAsLiked(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.LikedGames.MarkAsLikedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsLiked",
                $"User {LoggedUser.Name} mark as like game {game.Name}");
            UpdateList();
        }
        private async Task UnMarkAsLiked(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.LikedGames.UnMarkAsLikedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsLiked",
                $"User {LoggedUser.Name} unmark as like game {game.Name}");
            UpdateList();
        }
        #endregion
        #region Played
        private async Task MarkAsPlayed(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.PlayedGames.MarkAsPlayedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsPlayed",
                $"User {LoggedUser.Name} mark as played game {game.Name}");
            UpdateList();
        }
        private async Task UnMarkAsPlayed(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.PlayedGames.UnMarkAsPlayedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsPlayed",
                $"User {LoggedUser.Name} unmark as played game {game.Name}");
            UpdateList();
        }
        #endregion
        #region Watch
        private async Task MarkAsWatch(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.WatchGames.MarkAsWatchAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsWatch",
                $"User {LoggedUser.Name} mark as Watch game {game.Name}");
            UpdateList();
        }
        private async Task UnMarkAsWatch(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.WatchGames.UnMarkAsWatchAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsWatch",
                $"User {LoggedUser.Name} unmark as Watch game {game.Name}");
            UpdateList();
        }
        #endregion
        private async Task UpdateList()
        {
            await Update.InvokeAsync();
        }

        private async Task NavigateToGame()
        {
            Navigator.NavigateTo($"game/{item.Id}");
        }
    }
}
