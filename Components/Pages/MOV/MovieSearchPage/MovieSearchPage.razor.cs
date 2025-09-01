using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using System.Globalization;
using System.Text.RegularExpressions;
using ZetaCommon.Auth;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Models;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Data.MOV;
using ZetaDashboard.Services;
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
        private IReadOnlyCollection<Data.MOV.Genre> SelectedGenres = new List<Data.MOV.Genre>();
        private List<Watch_provider> SelectedProviders = new List<Watch_provider>();
        private Order SelectedOrder = MovieData.SortsBy[3];

        private Dictionary<string, string> QueryParams = new Dictionary<string, string>();

        private string _searchByName;

        private List<MovieModel> SeenMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> LikedMovieList { get; set; } = new List<MovieModel>();
        private List<MovieModel> WatchMovieList { get; set; } = new List<MovieModel>();
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
        }
        protected override async Task OnParametersSetAsync()
        {
            GetParamsQueryAndList();
            //GetList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GetList();
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
            //datagridLoading = true;
            //await InvokeAsync(StateHasChanged);
            _pag = 1;
            DataList.Clear();

            //var url = "search-movie/?";
            //foreach (var inn in QueryParams)
            //{
            //    url += $"&{inn.Key}={inn.Value}";
            //}
            //Navigator.NavigateTo(url, false, false);

            if (string.IsNullOrEmpty(SelectedCategory))
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser,QueryParams));
            }
            else if (SelectedCategory == "nowplaying")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "popular")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "toprated")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTopRatedAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "upcoming")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByUpcomingAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "search")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNameAsync(_searchByName, _pag, LoggedUser));
            }

            SeenMovieList = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            LikedMovieList = DController.GetData(await ApiService.LikedMovies.GetAllLikedMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();
            WatchMovieList = DController.GetData(await ApiService.WatchMovies.GetAllWatchMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();


            //datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

      
        private async Task LoadMore()
        {
            _pag++;

            if (string.IsNullOrEmpty(SelectedCategory))
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser, QueryParams)));
            }
            else if (SelectedCategory == "nowplaying")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "popular")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "toprated")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTopRatedAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "upcoming")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByUpcomingAsync(_pag, LoggedUser)));
            }
            else if (SelectedCategory == "search")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNameAsync(_searchByName, _pag, LoggedUser)));
            }

        }

        
        

        private CancellationTokenSource? _cts;
        private async Task OnQueryChanged(string text)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                _searchByName = text;
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
            GetList();
        }
    }
}
