using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
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
        [Parameter]
        public string mode { get; set; }
        [Parameter]
        public string name { get; set; }

        #region Injects
        [Inject] BaseService ApiService { get; set; }
        [Inject] HttpService HttpApiService { get; set; }
        [Inject] DataController DController { get; set; }
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private CommonServices CService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider Auth { get; set; } = default!;
        [Inject] private NavigationManager Navigator { get; set; } = default!;
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
        private IReadOnlyCollection<Genre> SelectedGenres = new List<Genre>();
        private List<Watch_provider> SelectedProviders = new List<Watch_provider>();
        private Order SelectedOrder = MovieData.SortsBy[3];

        private Dictionary<string, string> QueryParams = new Dictionary<string, string>();

        private string _searchByName;

        private List<MovieModel> SeenMovieList { get; set; } = new List<MovieModel>();
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
            GetList();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        #endregion
        #region CRUD
        #region GET
        private async Task GetList()
        {
            //datagridLoading = true;
            //await InvokeAsync(StateHasChanged);
            _pag = 1;

            if (mode == "nowplaying")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingAsync(_pag, LoggedUser));
            }
            else if (mode == "popular")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularAsync(_pag, LoggedUser));
            }
            else if (mode == "toprated")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTopRatedAsync(_pag, LoggedUser));
            }
            else if (mode == "upcoming")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByUpcomingAsync(_pag, LoggedUser));
            }
            else if (mode == "search")
            {
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNameAsync(_searchByName,_pag, LoggedUser));
            }
            else{
                DataList = await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser, QueryParams));
            }

            SeenMovieList = DController.GetData(await ApiService.SeenMovies.GetAllSeenMoviesByUserIdAsync(LoggedUser)).Result ?? new List<MovieModel>();


            //datagridLoading = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
        #endregion

      
        private async Task LoadMore()
        {
            _pag++;
            
            if (mode == "nowplaying")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNowPlayingAsync(_pag, LoggedUser)));
            }
            else if (mode == "popular")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByPopularAsync(_pag, LoggedUser)));
            }
            else if (mode == "toprated")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByTopRatedAsync(_pag, LoggedUser)));
            }
            else if (mode == "upcoming")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByUpcomingAsync(_pag, LoggedUser)));
            }
            else if (mode == "search")
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllMoviesByNameAsync(_searchByName,_pag, LoggedUser)));
            }
            else
            {
                DataList.AddRange(await DController.GetData(await HttpApiService.Movies.GetAllDiscoverMoviesAsync(_pag, LoggedUser, QueryParams)));
            }
        }

        private void SelectProviders(Watch_provider provider)
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
        }
        private async Task SearchMovies()
        {
            QueryParams = new Dictionary<string, string>();
            var querygenres = "";
            if(SelectedGenres != null && SelectedGenres.Count > 0)
            {
                foreach (var i in SelectedGenres)
                {
                    querygenres += i.Id + ",";
                }
                querygenres = querygenres.Substring(0, querygenres.Length - 1);
                QueryParams.Add("with_genres", querygenres);
            }

            var queryproviders = "";
            if (SelectedProviders != null && SelectedProviders.Count > 0)
            {
                foreach (var i in SelectedProviders)
                {
                    queryproviders += i.provider_id + ",";
                }
                queryproviders = queryproviders.Substring(0, queryproviders.Length - 1);
                QueryParams.Add("with_watch_providers", queryproviders);
                QueryParams.Add("watch_region", "ES");
            }

            if(SelectedOrder != null)
            {
                QueryParams.Add("sort_by", SelectedOrder.eng_order);
            }

            GetList();
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
    }
}
