using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using ZetaCommon.Auth;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Data.MOV;
using ZetaDashboard.Services;
using static MongoDB.Driver.WriteConcern;
using static ZetaDashboard.Common.ZDB.Models.UserModel;

namespace ZetaDashboard.Components.Pages.MOV.MovieSearchPage
{
    public partial class MovieSearchPage
    {
        
        public string mode { get; set; }
        [Parameter]
        public string urlparams { get; set; }
        

        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
        [Inject] private IBrowserViewportService BrowserViewportService { get; set; }
        [Inject] private IJSRuntime JS { get; set; } = default!;
        #endregion
        #region Vars
        #region Global
        private UserModel LoggedUser { get; set; }
        private UserPermissions ThisPage { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Visor
        };
        private UserPermissions ThisPageEdit { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Editor
        };
        private UserPermissions ThisPageAdmin { get; set; } = new UserPermissions()
        {
            Code = "mov",
            UserType = EUserPermissionType.Admin
        };
        #endregion

        private List<MovieModel> DataList { get; set; } = new List<MovieModel>();

        private int _pag = 1;
        private int _maxpag = 1;
        private IReadOnlyCollection<Data.MOV.Genre> SelectedGenres = new List<Data.MOV.Genre>();
        private List<Watch_provider> SelectedProviders = new List<Watch_provider>();
        private Order SelectedOrder = MovieData.SortsBy[3];

        private Dictionary<string, string> QueryParams = new Dictionary<string, string>();

        private string _searchByName;

        private List<MovieModel> SeenMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> LikedMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> WatchMovieList { get; set; } = new List<MovieModel>();

        Dictionary<string, string> Paramss = new Dictionary<string, string>();

        private bool IsLoading { get; set; } = true;

        private int Maxpages = 0;
        private bool IsPc = true;
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
                $"Entrando en SeachMovie",
                $"Entrando en SeachMovie; mode {mode}",
                Common.Mongo.ResponseStatus.Ok
                );
            await ApiService.Audits.InsertAsync(audit);

            var x = await BrowserViewportService.GetCurrentBreakpointAsync();
            IsPc = (x >= Breakpoint.Md);
            GetParamsQueryAndList();
        }
        protected override async Task OnParametersSetAsync()
        {
            //GetList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //GetList();
                StateHasChanged();
            }
        }
        #endregion
        private async Task GetParamsQueryAndList()
        {
            var uri = Navigator.ToAbsoluteUri(Navigator.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("mode", out var modevalue))
                SelectedCategory = modevalue;
            
            if (query.TryGetValue("page", out var page))
                _pag = Int32.Parse(page);

            if (query.TryGetValue("search", out var searchValue))
                _searchByName = searchValue;

            if (query.TryGetValue("selectedorder", out var selectedorderValue))
                SelectedOrder = MovieData.SortsBy.FirstOrDefault(x => x.eng_order == selectedorderValue);

            if (query.TryGetValue("watch_providers", out var watch_providersValue))
            {
                var x = watch_providersValue.ToString().Split(",");
                foreach(var y in x)
                {
                    SelectedProviders.Add(MovieData.WhatProviders.FirstOrDefault(z => z.provider_id.ToString() == y));
                }
            }
            
            if (query.TryGetValue("genres", out var genresValue))
            {
                var x = genresValue.ToString().Split(",");
                var list = new List<Data.MOV.Genre>();
                foreach (var y in x)
                {
                    list.Add(MovieData.Genres.FirstOrDefault(z => z.Id.ToString() == y));
                }
                SelectedGenres = list;
            }



            //if (query.TryGetValue("selectedorder", out var selectedorderValue))
            //    SelectedOrder = MovieData.SortsBy.FirstOrDefault(x => x.eng_order == selectedorderValue);

            FilterBy();
        }
        private async Task FilterBy()
        {
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                if (SelectedCategory == "nowplaying")
                {
                    mode = "nowplaying";
                }
                else if (SelectedCategory == "popular")
                {
                    mode = "popular";
                }
                else if (SelectedCategory == "toprated")
                {
                    mode = "toprated";
                }
                else if (SelectedCategory == "upcoming")
                {
                    mode = "upcoming";
                }
                GetList();
                return;
            }


            QueryParams.Clear();

            QueryParams.Add("sort_by", SelectedOrder.eng_order);

            if (SelectedProviders.Count > 0)
            {
                var watch_providers = "";
                foreach(var watch in SelectedProviders)
                {
                    watch_providers += watch.provider_id + ",";
                }
                QueryParams.Add("with_watch_providers", watch_providers.Substring(0, watch_providers.Length - 1));
                QueryParams.Add("watch_region", "ES");
            }

            if (SelectedGenres.Count > 0)
            {
                var genres = "";
                foreach (var genre in SelectedGenres)
                {
                    genres += genre.Id + ",";
                }

                QueryParams.Add("with_genres", genres.Substring(0, genres.Length - 1));
            }


            GetList();
        }
        #region CRUD
        #region GET
        private async Task GetList()
        {
            DataList.Clear();
            IsLoading = true;
            await InvokeAsync(StateHasChanged);

            if (string.IsNullOrEmpty(SelectedCategory))
            {
                var x = await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMovieModelAsync(_pag, LoggedUser, QueryParams)); ;
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else if (SelectedCategory == "nowplaying")
            {
                    var x = (await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingMovieAsync(_pag, LoggedUser)));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else if (SelectedCategory == "popular")
            {
                   var x = (await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularMovieAsync(_pag, LoggedUser)));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else if (SelectedCategory == "toprated")
            {
                   var x = (await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTopRatedMovieAsync(_pag, LoggedUser)));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else if (SelectedCategory == "upcoming")
            {
                   var x = (await DController.GetData(await HttpApiService.Movies.GetAllMoviesByUpcomingMovieAsync(_pag, LoggedUser)));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else if (SelectedCategory == "search")
            {
                var x =  await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNameMovieAsync(_searchByName, _pag, LoggedUser));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }
            else
            {
                var x = (await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingMovieAsync(_pag, LoggedUser)));
                DataList = x.Results;
                _maxpag = x.TotalPages;
            }

                SeenMovieList = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            LikedMovieList = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            WatchMovieList = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();


            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

      

        private CancellationTokenSource? _cts;
        private async Task OnQueryChanged(string text)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                _searchByName = text;
                await SetUrl();
                GetList();
            }
            catch (OperationCanceledException)
            {
                // esperado al teclear rápido
            }
            catch (Exception ex)
            {
                // loguea si quieres
                Console.WriteLine($"Search error: {ex.Message}");
            }
            
        }

        private async Task SetUrl(bool paage = true)
        {
            Paramss.Clear();
            if(paage)
                _pag = 1;

            Paramss.Add(key: "page", value: _pag.ToString());
            if (!string.IsNullOrEmpty(_searchByName))
            {
                Paramss.Add(key: "search", value:_searchByName);
            }
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                Paramss.Add(key: "mode", value: SelectedCategory);
            }
            if (!string.IsNullOrEmpty(SelectedOrder.eng_order))
            {
                Paramss.Add(key: "selectedorder", value: SelectedOrder.eng_order);
            }
            if (SelectedProviders.Count > 0)
            {
                var aux = string.Join(",", SelectedProviders.Select(x => x.provider_id));

                Paramss.Add(key: "watch_providers", value: aux);
            }
            if (SelectedGenres.Count > 0)
            {
                var aux = string.Join(",", SelectedGenres.Select(x => x.Id));
                Paramss.Add(key: "genres", value: aux);
            }
            await JS.InvokeVoidAsync("replaceQuery", Paramss);

        }

        public string GetTitle(string mode) => mode switch
        {
            null => "📍 Descubre Peliculas",
            "" => "📍 Descubre Peliculas",
            "discover" => "📍 Descubre Peliculas",
            "nowplaying" => "⌛  Ahora mismo",
            "popular" => "🔥 Populares",
            "toprated" => "📈 Mas valoradas",
            "upcoming" => "🆕  Nuevas",
            "search" => "🔎 Buscar"
        };

        private string SelectedCategory;

        private async Task SelectedCategoryChanged(string cat)
        {
            SelectedCategory = cat;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, _cts.Token);

                await SetUrl();
                await FilterBy();
            }
            catch (TaskCanceledException)
            {
            }
            
        }
        private async Task SelectedOrderChanged(Order order)
        {
            SelectedOrder = order;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, _cts.Token);

                await SetUrl();
                await FilterBy();
            }
            catch (TaskCanceledException)
            {
            }

        }
        private async Task SelectProviders(Watch_provider provider)
        {
            if (SelectedProviders.Contains(provider))
            {
                SelectedProviders.Remove(provider);
            }
            else
            {
                SelectedProviders.Add(provider);
            }
            StateHasChanged();


            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Delay(1000, _cts.Token);
                await SetUrl();
                await FilterBy();
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task SelectGenresChanged(IReadOnlyCollection<Data.MOV.Genre> genres)
        {
            SelectedGenres = genres.ToList();
            StateHasChanged();


            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Delay(1000, _cts.Token);
                await SetUrl();
                await FilterBy();
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task SearchByTitleMovies()
        {
            SelectedCategory = "search";
            _searchByName = "";
        }
        private async Task CancelSearchByTitleMovies()
        {
            SelectedCategory = null;
            _searchByName = "";
            await SetUrl();
            GetList();
        }

        private async Task PagChanged(int pag)
        {
            _pag = pag;
            await SetUrl(false);
            await GetList();
            await JS.InvokeVoidAsync("scrollToTop");
        }
    }
}
