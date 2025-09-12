using System.Text.Json;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.Http;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;

namespace ZetaDashboard.Common.Services
{
    public partial class HttpService
    {
        public class GameService : HttpRepositoryBase
        {
            private readonly List<string> thispage = new() { "gms" };

            private readonly JsonSerializerOptions _json = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            private const string AuditsRoute = "/audits";

            public string _dato = "game";
            public string _datos = "games";
            public string _ellaDato = "el game";
            public string _loslasDatos = "los games";

            public GameService(HttpClient http) : base(http) { }
            #region Get
            public async Task<ApiResponse<List<RawgGame>>> GetAllGamesAsync(UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<List<RawgGame?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<GameModel>>("games", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        var res = JsonSerializer.Deserialize<GameModel>(raw, _json);
                        response.Data = res.Results;
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
                    return new ApiResponse<List<RawgGame>>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<GameModel>> GetSearchGameModelAsync(Dictionary<string,string> paramss, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<GameModel>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    string queryparamsstring = $"?page_size=20";
                    //se vienen
                    //string queryparamsstring = $"?&ordering=&page_size=20";
                    //populares
                    //string queryparamsstring = $"?dates=2025-06-01,2025-09-11&ordering=-rating&page_size=20";
                    //juegos del momento
                    //string queryparamsstring = $"?&ordering=&page_size=20";
                    if (paramss != null)
                    {
                        foreach (var inn in paramss)
                        {
                            queryparamsstring += $"&{inn.Key}={inn.Value}";
                        }
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<GameModel>>($"games{queryparamsstring}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<GameModel>(raw, _json);
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
                    return new ApiResponse<GameModel>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<RawgGameDetail>> GetGameModelByIdAsync(string gameId, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<RawgGameDetail>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<RawgGameDetail>>($"games/{gameId}", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<RawgGameDetail>(raw, _json);
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
                    return new ApiResponse<RawgGameDetail>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }
            public async Task<ApiResponse<RawgScreenshotsResponse>> GetGameScreenShotsAsync(string gameId, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<RawgScreenshotsResponse>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<RawgScreenshotsResponse>>($"games/{gameId}/screenshots", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<RawgScreenshotsResponse>(raw, _json);
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
                    return new ApiResponse<RawgScreenshotsResponse>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<RawgAchievementsResponse>> GetGameAchievementssAsync(string gameId, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<RawgAchievementsResponse>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<RawgAchievementsResponse>>($"games/{gameId}/achievements", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<RawgAchievementsResponse>(raw, _json);
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
                    return new ApiResponse<RawgAchievementsResponse>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<RawgVideosResponse>> GetGameVideosAsync(string gameId, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<RawgVideosResponse>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<RawgVideosResponse>>($"games/{gameId}/movies", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<RawgVideosResponse>(raw, _json);
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
                    return new ApiResponse<RawgVideosResponse>
                    {
                        Result = ResponseStatus.InternalError,
                        Message = $"Ha ocurrido un error al recuperar {_loslasDatos}",
                    };
                }
            }

            public async Task<ApiResponse<GameModel>> GetGameSeriesAsync(string gameId, UserModel loggeduser, CancellationToken ct = default)
            {
                var response = new ApiResponse<GameModel>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response!;
                    }

                    // Opción A) Tu API devuelve ApiResponse<List<AuditModel>>
                    var (ok, apiRes, error, raw) = await TryGetAsync<ApiResponse<GameModel>>($"games/{gameId}/game-series", ct);

                    if (ok && apiRes is not null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = JsonSerializer.Deserialize<GameModel>(raw, _json);
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
                    return new ApiResponse<GameModel>
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
