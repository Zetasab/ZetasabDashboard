using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using ZetaCommon.Auth;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Handlers;
using ZetaDashboard.Services;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.GMS.DetailedGamePage
{
    public partial class DetailedGamePage
    {
        [Parameter] public string gameId { get; set; } = "";
        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
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

        #region Vars
        private RawgGameDetail GameDetail { get; set; } = new RawgGameDetail();
        private List<RawgGame> PlayedGames { get; set; } = new List<RawgGame>();
        private List<RawgGame> LikedGames { get; set; } = new List<RawgGame>();
        private List<RawgGame> WatchGames { get; set; } = new List<RawgGame>();

        private List<RawgGame> SameSeries { get; set; } = new List<RawgGame> { };
        private List<RawgVideo> VideosGame { get; set; } = new List<RawgVideo> { };
        private List<RawgScreenshot> ScreenshotsGame { get; set; } = new List<RawgScreenshot> { };
        private List<RawgAchievement> AchievementsGame { get; set; } = new List<RawgAchievement> { };

        private int videoSelected = 0;
        private bool IsLoading = true;
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
                $"Entrando en game",
                $"Entrando en game; game {gameId}",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);

        }
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await JS.InvokeVoidAsync("scrollToTop");
                //IsLoading = true;
                await GetGame();

                await JS.InvokeVoidAsync("initGallery");
            }
            catch (Exception ex)
            {

            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        #endregion

        #region Get
        private async Task GetGame()
        {
            IsLoading = true;
            await InvokeAsync(StateHasChanged);

            GameDetail = await DController.GetData(await HttpApiService.Games.GetGameModelByIdAsync(gameId.ToString(), LoggedUser));

            PlayedGames = DController.GetData(await ApiService.PlayedGames.GetAllPlayedGamesByUserIdAsync(LoggedUser)).Result ?? new List<RawgGame>();
            LikedGames = DController.GetData(await ApiService.LikedGames.GetAllLikedGamesByUserIdAsync(LoggedUser)).Result ?? new List<RawgGame>();
            WatchGames = DController.GetData(await ApiService.WatchGames.GetAllWatchGamesByUserIdAsync(LoggedUser)).Result ?? new List<RawgGame>();

            //GameDetail = await DController.GetData(await HttpApiService.Games.GetGameModelByIdAsync(gameId, LoggedUser));
            
            if(GameDetail.ScreenshotsCount > 0)
            {
                   var x = await DController.GetData(await HttpApiService.Games.GetGameScreenShotsAsync(GameDetail.Id.ToString(),LoggedUser));
                    ScreenshotsGame = x.Results;
            }
            if (GameDetail.MoviesCount > 0)
            {
                var x = await DController.GetData(await HttpApiService.Games.GetGameVideosAsync(GameDetail.Id.ToString(), LoggedUser));
                VideosGame = x.Results;
            }
            if (GameDetail.GameSeriesCount > 0)
            {
                var x = await DController.GetData(await HttpApiService.Games.GetGameSeriesAsync(GameDetail.Id.ToString(), LoggedUser));
                SameSeries = x.Results;
            }
            if (GameDetail.AchievementsCount > 0)
            {
               var x = await DController.GetData(await HttpApiService.Games.GetGameAchievementssAsync(GameDetail.Id.ToString(), LoggedUser));
                AchievementsGame = x.Results;
            }

            await CreateGraph();
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        private Color GetScoreColor(double voteAverage)
        {
            var percent = voteAverage * 10; // 0-100
            if (percent >= 70) return Color.Success;   // verde
            if (percent >= 40) return Color.Warning;   // amarillo
            return Color.Error;                        // rojo
        }


        #region Seen
        private async Task MarkAsSeen(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.PlayedGames.MarkAsPlayedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsSeen",
                $"User {LoggedUser.Name} mark as played game {game.Name}");
            GetGame();
        }
        private async Task UnMarkAsSeen(RawgGame game)
        {
            _ = DController.UpdateData(await ApiService.PlayedGames.UnMarkAsPlayedAsync(game, LoggedUser),
                LoggedUser,
                "UnMarkAsSeen",
                $"User {LoggedUser.Name} unmark as played game {game.Name}");

            GetGame();
        }
        #endregion
        #region Like
        private async Task MarkAsLiked(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.LikedGames.MarkAsLikedAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsLiked",
                $"User {LoggedUser.Name} mark as like game {game.Name}");
            GetGame();
        }
        private async Task UnMarkAsLiked(RawgGame game)
        {
            _ = DController.UpdateData(await ApiService.LikedGames.UnMarkAsLikedAsync(game, LoggedUser),
                LoggedUser,
                "UnMarkAsLiked",
                $"User {LoggedUser.Name} unmark as like game {game.Name}");

            GetGame();
        }
        #endregion
        #region Watch
        private async Task MarkAsWatch(RawgGame game)
        {
            _ = await DController.UpdateData(await ApiService.WatchGames.MarkAsWatchAsync(game, LoggedUser),
                LoggedUser,
                "MarkAsWatch",
                $"User {LoggedUser.Name} mark as watch game {game.Name}");
            GetGame();
        }
        private async Task UnMarkAsWatch(RawgGame game)
        {
            _ = DController.UpdateData(await ApiService.WatchGames.UnMarkAsWatchAsync(game, LoggedUser),
                LoggedUser,
                "UnMarkAsWatch",
                $"User {LoggedUser.Name} unmark as watch game {game.Name}");

            GetGame();
        }
        #endregion

        private async Task PreviusVideo()
        {
            if (videoSelected == 0)
            {
                videoSelected = VideosGame.Count -1;
            }
            else
            {
                videoSelected--;
            }
            await InvokeAsync(StateHasChanged);
        }
        private async Task NextVideo()
        {
            if (videoSelected == VideosGame.Count -1)
            {
                videoSelected = 0;
            }
            else
            {
                videoSelected++;
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task GoBack()
        {
            await JS.InvokeVoidAsync("goBack");
        }

        private RawgGame Transform(RawgGameDetail detail)
        {
            var newgame = new RawgGame()
            {
                Id = detail.Id,
                Slug = detail.Slug,
                Name = detail.Name,
                Released = detail.Released,
                BackgroundImage = detail.BackgroundImage,
                Rating = detail.Rating,
                RatingTop = detail.RatingTop ?? 0,
                Ratings = new List<RawgRating>()
                {
                    
                },
                    RatingsCount = detail.RatingsCount ?? 0,
                    ReviewsTextCount = detail.ReviewsTextCount ?? 0,
                    Added = detail.Added ?? 0,
                    AddedByStatus = detail.AddedByStatus,
                    Metacritic =detail.Metacritic,
                    Playtime =detail.Playtime ?? 0,
                    SuggestionsCount =detail.SuggestionsCount ?? 0,
                    Updated =detail.Updated,

                    ReviewsCount =detail.ReviewsCount ?? 0,
                    SaturatedColor =detail.SaturatedColor,
                    DominantColor =detail.DominantColor,

                    Platforms = new List<PlatformEntry>
                    {
                   
                    },

                    ParentPlatforms = new List<ParentPlatformRef>
                    {
                    },

                    Genres = new List<Common.GMS.Genre>
                    {
                    },

                    Stores = new List<StoreEntry>
                    {
                        
                    },

                    Tags = new List<Tag>
                    {
                    },

                    EsrbRating = detail.EsrbRating,

                    ShortScreenshots = new List<ShortScreenshot>
                    {
                    }
            };
            return newgame;
        }




        private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        // Escríbelos ya "normalizados"
        ["steam"]                 = "steam",
        ["playstation store"]     = "playstation",
        ["playstation"]           = "playstation",
        ["ps"]                    = "playstation",
        ["xbox-store"]            = "xstate",
        ["xbox-360"]              = "xstate",
        ["xbox"]                  = "xstate",
        ["app store"]             = "appstore",
        ["apple appstore"]        = "appstore",
        ["gog"]                   = "gogdotcom",
        ["gog.com"]               = "gogdotcom",
        ["nintendo store"]        = "nintendo",
        ["nintendo"]              = "nintendo",
        ["google play"]           = "googleplay",
        ["google play store"]     = "googleplay",
        ["itch.io"]               = "itchdotio",
        ["itch"]                  = "itchdotio",
        ["epic games"]            = "epicgames",
        ["epic games store"]      = "epicgames",
    };

    /// <summary>
    /// Returns a CDN URL for the store icon (SVG). Optional color without '#'.
    /// Examples: "Steam" -> https://cdn.simpleicons.org/steam
    ///           "PlayStation Store" -> https://cdn.simpleicons.org/playstation
    /// </summary>
    public static string GetStoreIconUrl(string? name, string? colorHex = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "https://cdn.simpleicons.org/pcgamingwiki"; // fallback genérico

        var key = Normalize(name);

        if (!Map.TryGetValue(key, out var slug))
        {
            // Fallback heurístico por contención de texto
            if      (key.Contains("steam"))        slug = "steam";
            else if (key.Contains("playstation"))  slug = "playstation";
            else if (key.Contains("xbox"))         slug = "xstate";
            else if (key.Contains("xbox360"))         slug = "xstate";
            else if (key.Contains("xbox-store"))         slug = "xstate";
            else if (key.Contains("nintendo"))     slug = "nintendo";
            else if (key.Contains("epic"))         slug = "epicgames";
            else if (key.Contains("gog"))          slug = "gogdotcom";
            else if (key.Contains("google") && key.Contains("play")) slug = "googleplay";
            else if (key.Contains("app") && key.Contains("store"))   slug = "appstore";
            else if (key.Contains("itch"))         slug = "itchdotio";
            else                                    slug = "simpleicons"; // último recurso
        }

        var baseUrl = "https://cdn.simpleicons.org/";
        return string.IsNullOrWhiteSpace(colorHex)
            ? $"{baseUrl}{slug}"
            : $"{baseUrl}{slug}/{colorHex.TrimStart('#')}";
    }

    private static string Normalize(string s)
    {
        var t = s.Trim().ToLowerInvariant();
        t = t.Replace(".", "").Replace("_", " ").Replace("-", " ");
        t = Regex.Replace(t, @"\s+", " ");
        return t;
    }
        

        private bool DescriptionBool = true;

        public double[] data;
        public string[] labels;

        private ChartOptions options = new ChartOptions
        {
            ChartPalette = new string[] { "rgba(11,186,131,1)", "rgba(50,153,255,1)", "rgba(255,168,0,1)", "rgba(246,78,98,1)" }
        };
        private async Task CreateGraph()
        {
            var _doubles = new List<double>();
            var _strings = new List<string>();
            foreach (var item in GameDetail.Ratings)
            {
                _doubles.Add(item.Count);
                _strings.Add(item.Title.ToUpper());
            }
            data = _doubles.ToArray();
            labels = _strings.ToArray();
        }
        private string GetLabelTitle(string text)
        {
            if (text.Equals("EXCEPTIONAL"))
            {
                return "🔥 Genial";
            }
            else if (text.Equals("RECOMMENDED"))
            {
                return "👍 Recomendable";
            }
            else if (text.Equals("SKIP"))
            {
                return "☠ Omitible";
            }
            else if (text.Equals("MEH"))
            {
                return "😕 Meh";
            }
            return text;
        }

    }
}
