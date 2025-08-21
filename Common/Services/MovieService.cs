using System.Text.Json;
using ZetaDashboard.Common.Http;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;

namespace ZetaDashboard.Common.Services
{
    public partial class HttpService
    {
        public class MovieService : HttpRepositoryBase
        {
            private readonly List<string> thispage = new() { "mov" };

            private readonly JsonSerializerOptions _json = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            private const string AuditsRoute = "/audits";

            public string _dato = "auditoria";
            public string _datos = "auditorias";
            public string _ellaDato = "la auditoria";
            public string _loslasDatos = "las auditorias";

            public MovieService(HttpClient http) : base(http) { }
            #region Get
            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesAsync(UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>("search/movie?query=spider&page=3", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;
                        
                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesByNameAsync(string name,int page,UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"search/movie?query={name}&page={page}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesByNowPlayingAsync(int page,UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"movie/now_playing?language=es-ES&page={page}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesByPopularAsync(int page, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"movie/popular?language=es-ES&page={page}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesByTopRatedAsync(int page, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"movie/top_rated?language=es-ES&page={page}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<List<MovieModel>>> GetAllMoviesByUpcomingAsync(int page, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"movie/upcoming?language=es-ES&page={page}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<List<MovieModel>>> GetAllDiscoverMoviesAsync(int page, UserModel loggeduser, Dictionary<string, string> queryparams = null, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }
                    string queryparamsstring = $"?page={page}&language=es-ES";
                    if (queryparams != null)
                    {
                        foreach (var inn in queryparams)
                        {
                            queryparamsstring += $"&{inn.Key}={inn.Value}";
                        }
                    }
                    
                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<List<MovieModel>>>($"discover/movie{queryparamsstring}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<MovieListModel>(raw, _json).Results;
                        return response;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                        return response!;
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    return new ApiResponse<List<MovieModel>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            #endregion
        }
    }
}
