using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Text;
using ZetaCommon.Auth;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.GMS.PlayedGamesPage
{
    public partial class PlayedGamesPage
    {
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
        #endregion

        #region Global
        private UserModel LoggedUser { get; set; }
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


        #endregion
        private List<RawgGame> DataList { get; set; } = new List<RawgGame>();
        private List<RawgGame> DataBup { get; set; } = new List<RawgGame>();
        private List<RawgGame> PlayedGamesList { get; set; } = new List<RawgGame>();
        private List<RawgGame> LikedGamesList { get; set; } = new List<RawgGame>();
        private List<RawgGame> WatchGamesList { get; set; } = new List<RawgGame>();

        protected override async Task OnInitializedAsync()
        {
            LoggedUser = (Auth as CustomAuthenticationStateProvider).LoggedUser;
            CService.CheckPermissions(LoggedUser, ThisPage);
            var audit = new AuditModel(
                LoggedUser.Id,
                LoggedUser.Name,
                AuditWhat.See,
                $"Entrando en PlayedGames",
                $"Entrando en PlayedGames",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            await GetList();
        }

        private async Task GetList()
        {
            DataBup = await DController.GetData(await ApiService.PlayedGames.GetAllPlayedGamesByUserIdAsync(LoggedUser));
            DataList = DataBup.ToList();

            PlayedGamesList = await DController.GetData(await ApiService.PlayedGames.GetAllPlayedGamesByUserIdAsync(LoggedUser)) ?? new List<RawgGame>();
            LikedGamesList = await DController.GetData(await ApiService.LikedGames.GetAllLikedGamesByUserIdAsync(LoggedUser)) ?? new List<RawgGame>();
            WatchGamesList = await DController.GetData(await ApiService.WatchGames.GetAllWatchGamesByUserIdAsync(LoggedUser)) ?? new List<RawgGame>();

            await InvokeAsync(StateHasChanged);

        }

        

        private string _search = "";

        private CancellationTokenSource? _queryCts;
        private async void OnQueryChanged(string text)
        {
            _search = text;
            // cancela búsquedas anteriores (si el usuario sigue escribiendo)
            _queryCts?.Cancel();
            _queryCts = new CancellationTokenSource();
            var token = _queryCts.Token;

            // debounce suave
            try { await Task.Delay(100, token); } catch (TaskCanceledException) { return; }

            if (string.IsNullOrEmpty(text))
            {
                DataList = DataBup.ToList();
                StateHasChanged();
                return;
            }

            var q = Normalize(text);

            IEnumerable<RawgGame> result =
                string.IsNullOrEmpty(q)
                ? DataBup
                : DataBup.Where(item => Matches(item, q));

            // Refresca la lista mostrada
            DataList.Clear();
            foreach (var it in result)
                DataList.Add(it);

            StateHasChanged();
        }
        private static bool Matches(RawgGame item, string q)
        {
            if (item is null) return false;
            return Normalize(item.Name).Contains(q, StringComparison.Ordinal);
        }

        // Normaliza: trim, lower-invariant, quita acentos
        private static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            var formD = s.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder(formD.Length);

            foreach (var c in formD)
            {
                var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
