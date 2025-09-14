using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text.Json;
using ZetaCommon.Auth;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Data.GMS;
using ZetaDashboard.Data.MOV;
using ZetaDashboard.Services;
using static System.Formats.Asn1.AsnWriter;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.GMS.SearchGamesPage
{
    public partial class SearchGamesPage
    {
        #region Vars
        #region Injects
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;


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

        private List<RawgGame> GameList { get; set; } = new List<RawgGame>();
        private List<RawgGame> LikedGameList { get; set; } = new List<RawgGame>();
        private List<RawgGame> PlayedGameList { get; set; } = new List<RawgGame>();
        private List<RawgGame> WatchedGameList { get; set; } = new List<RawgGame>();

        private int _pag = 1;
        private int _maxpag = 0;
        private string _search = "";
        private List<PlatformCategory> _platforms { get; set; } = new List<PlatformCategory>();
        private List<GameGenre> _genres { get; set; } = new List<GameGenre>();
        private List<GameTag> _tags { get; set; } = new List<GameTag>();
        private GameSorting _sorting { get; set; } = GameData.Sortings[0];

        Dictionary<string, string> Paramss = new Dictionary<string, string>();

        private bool IsLoading { get; set; } = true;

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
                $"Entrando en Home games",
                $"Entrando en Home games",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);
            await GetParamsQueryAndList();
            await GetListActions();

            await InvokeAsync(StateHasChanged);

        }
        #endregion

        private async Task GetParamsQueryAndList()
        {
            var uri = Navigator.ToAbsoluteUri(Navigator.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("search", out var name))
                _search = name;
            if (query.TryGetValue("page", out var page))
                _pag =  Int32.Parse(page);
            if (query.TryGetValue("parent_platforms", out var platform))
            {
                var x = platform.ToString().Split(",");
                foreach (var y in x)
                {
                    _platforms.Add(GameData.ParentPlatforms.FirstOrDefault(z => z.Id.ToString() == y));
                }
                
            }
                
            if (query.TryGetValue("genres", out var genre))
            {
                var x = genre.ToString().Split(",");
                foreach (var y in x)
                {
                    _genres.Add(GameData.Games_Genres.FirstOrDefault(z => z.Slug.ToString() == y));
                }
            }
            if (query.TryGetValue("tags", out var tag))
            {
                var x = tag.ToString().Split(",");
                foreach (var y in x)
                {
                    _tags.Add(GameData.TagList.FirstOrDefault(z => z.Slug.ToString() == y));
                }
            }
            if (query.TryGetValue("ordering", out var sort))
            {
                _sorting = GameData.Sortings.FirstOrDefault(z => z.Value.ToString() == sort);
            }


            await SetUrl(false);
            await UpdateList();
        }

        #region GET
        private async Task GetListActions()
        {
            LikedGameList = await DController.GetData(await ApiService.LikedGames.GetAllLikedGamesByUserIdAsync(LoggedUser));
            PlayedGameList = await DController.GetData(await ApiService.PlayedGames.GetAllPlayedGamesByUserIdAsync(LoggedUser));
            WatchedGameList = await DController.GetData(await ApiService.WatchGames.GetAllWatchGamesByUserIdAsync(LoggedUser));

            await InvokeAsync(StateHasChanged);
        
        }
        private async Task UpdateList()
        {
            GameList.Clear();
            IsLoading = true;
            await InvokeAsync(StateHasChanged);

            var x = await DController.GetData(await HttpApiService.Games.GetSearchGameModelAsync(Paramss,LoggedUser));
            GameList = x.Results;
            _maxpag = x.Count / 2;
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        


        #endregion

        #region Filter

        private CancellationTokenSource? _cts;
        private async void OnQueryChanged(string text)
        {
            _search = text;
            // cancela búsquedas anteriores (si el usuario sigue escribiendo)
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // debounce suave
            try 
            {
                await SetUrl();
                await UpdateList();
            } 
            catch (TaskCanceledException) { return; }

            
            StateHasChanged();
        }
        

        private async Task SelectedPlatformChanged(IEnumerable<PlatformCategory?> platforms)
        {
            _platforms = platforms.ToList();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _cts.Token);

                await SetUrl();
                await UpdateList();
            }
            catch (TaskCanceledException)
            {
            }

        }
        private async Task SelectedGenresChanged(IEnumerable<GameGenre> genres)
        {
            _genres = genres.ToList();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _cts.Token);

                await SetUrl();
                await UpdateList();
            }
            catch (TaskCanceledException)
            {
            }

        }
        private async Task SelectedTagsChanged(IEnumerable<GameTag> tags)
        {
            _tags = tags.ToList();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _cts.Token);

                await SetUrl();
                await UpdateList();
            }
            catch (TaskCanceledException)
            {
            }

        }
        private async Task SelectedSortingChanged(GameSorting sorting)
        {
            _sorting = sorting;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _cts.Token);

                await SetUrl();
                await UpdateList();
            }
            catch (TaskCanceledException)
            {
            }

        }

        private async Task SetUrl(bool paage = true)
        {
            Paramss.Clear();
            if (paage)
                _pag = 1;

            Paramss.Add(key: "page", value: _pag.ToString());
            if (!string.IsNullOrEmpty(_search))
            {
                Paramss.Add(key: "search", value: _search);
            }
            if (_platforms.Count > 0)
            {
                var aux = string.Join(",", _platforms.Select(x => x.Id)).ToString().ToLower();
                Paramss.Add(key: "parent_platforms", value: aux);
            }
            if (_genres.Count > 0)
            {
                var aux = string.Join(",", _genres.Select(x => x.Slug)).ToString().ToLower();
                Paramss.Add(key: "genres", value: aux);
            }
            if (_tags.Count > 0)
            {
                var aux = string.Join(",", _tags.Select(x => x.Slug)).ToString().ToLower();
                Paramss.Add(key: "tags", value: aux);
            }

            if (_sorting != null)
            {
                Paramss.Add(key: "ordering", value: _sorting.Value);
            }
            await JS.InvokeVoidAsync("replaceQuery", Paramss);

        }
        private async Task PagChanged(int pag)
        {
            _pag = pag;
            await SetUrl(false);
            await UpdateList();
            await JS.InvokeVoidAsync("scrollToTop");
        }
        #endregion

    }
}
