using MongoDB.Driver;
using ZetaDashboard.Common.GMS;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class LikedGameService : MongoRepositoryBase<LikedGameModel>
        {
            private List<string> thispage = new List<string>() { "gms" };
            public LikedGameService(MongoContext context)
                : base(context, "gms_liked") { }

            public string _dato = "lista de jugados";
            public string _datos = "listas de jugados";
            public string _ellaDato = "la lista de jugados";
            public string _loslasDatos = "las listas de jugados";

            #region Get
            public async Task<ApiResponse<List<LikedGameModel>>> GetAllLikedGamesAsync(UserModel loggeduser)
            {
                ApiResponse<List<LikedGameModel?>> response = new ApiResponse<List<LikedGameModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser,UserModel.EUserPermissionType.Visor,thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var result = await FindAllAsync();
                    if (result != null)
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al obtener los proyectos";
                    Console.WriteLine(ex.ToString());
                }
                return response;
            }

            public async Task<ApiResponse<List<RawgGame>>> GetAllLikedGamesByUserIdAsync(UserModel loggeduser)
            {
                ApiResponse<List<RawgGame?>> response = new ApiResponse<List<RawgGame?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var filter = Builders<LikedGameModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);
                    if (result != null)
                    {
                        if (result.Count == 0)
                        {
                            LikedGameModel aux = new LikedGameModel()
                            {
                                Games = new List<RawgGame>(),
                                UserId = loggeduser.Id
                            };
                            await InsertAsync(aux);
                            result = await FindAllAsync(filter);
                        }
                        response.Result = ResponseStatus.Ok;
                        response.Data = result[0].Games;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = $"Error obteniendo {_loslasDatos}";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al obtener los proyectos";
                    Console.WriteLine(ex.ToString());
                }
                return response;
            }

            #endregion


            #region Post
            public async Task<ApiResponse<bool>> InsertLikedGameAsync(LikedGameModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();
                
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await InsertAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha insertado correctamente";
                }
                catch(Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al insertar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update

            public async Task <ApiResponse<bool>> MarkAsLikedAsync(RawgGame movie,UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var filter = Builders<LikedGameModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);

                    if(result.Count == 0)
                    {
                        LikedGameModel aux = new LikedGameModel()
                        {
                            Games = new List<RawgGame>(),
                            UserId = loggeduser.Id
                        };
                        await InsertAsync(aux);
                        result = await FindAllAsync(filter);
                    }

                    movie.Format();
                    result[0].Games.Add(movie);
                    await UpdateAsync(result[0]);

                    
                    response.Result = ResponseStatus.Ok;
                    response.Message = $"La pelicula se ha marcado como me gusta";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al editar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }

            public async Task<ApiResponse<bool>> UnMarkAsLikedAsync(RawgGame movie, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var filter = Builders<LikedGameModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);

                    if (result.Count == 0)
                    {
                        LikedGameModel aux = new LikedGameModel()
                        {
                            Games = new List<RawgGame>(),
                            UserId = loggeduser.Id
                        };
                        await InsertAsync(aux);
                        result = await FindAllAsync(filter);
                    }


                    result[0].Games.RemoveAll(x => x.Id == movie.Id);
                    await UpdateAsync(result[0]);


                    response.Result = ResponseStatus.Ok;
                    response.Message = $"La pelicula se ha quitado de me gusta";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al editar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }

            public async Task<ApiResponse<bool>> UpdateLikedGameAsync(LikedGameModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Editor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await UpdateAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al editar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeleteLikedGameAsync(LikedGameModel model, UserModel loggeduser)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Admin, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }
                    await DeleteAsync(model);
                    response.Result = ResponseStatus.Ok;
                    response.Message = $"{char.ToUpper(_ellaDato[0]) + _ellaDato.Substring(1).ToLower()} se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = $"Ha ocurrido un error al borrar {_ellaDato}";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }


    }
}
