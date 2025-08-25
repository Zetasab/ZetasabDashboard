using MongoDB.Driver;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.MOV;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class LikedMovieService : MongoRepositoryBase<LikedMovieModel>
        {
            private List<string> thispage = new List<string>() { "mov" };
            public LikedMovieService(MongoContext context)
                : base(context, "mov_liked") { }

            public string _dato = "lista de vistos";
            public string _datos = "listas de vistos";
            public string _ellaDato = "la lista de vistos";
            public string _loslasDatos = "laas listas de vistos";

            #region Get
            public async Task<ApiResponse<List<LikedMovieModel>>> GetAllLikedMoviesAsync(UserModel loggeduser)
            {
                ApiResponse<List<LikedMovieModel?>> response = new ApiResponse<List<LikedMovieModel?>>();
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

            public async Task<ApiResponse<List<MovieModel>>> GetAllLikedMoviesByUserIdAsync(UserModel loggeduser)
            {
                ApiResponse<List<MovieModel?>> response = new ApiResponse<List<MovieModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
                    {
                        response.Result = ResponseStatus.Unauthorized;
                        response.Message = "No tienes permisos";
                        return response;
                    }

                    var filter = Builders<LikedMovieModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);
                    if (result != null)
                    {
                        if (result.Count == 0)
                        {
                            LikedMovieModel aux = new LikedMovieModel()
                            {
                                Movies = new List<MovieModel>(),
                                UserId = loggeduser.Id
                            };
                            await InsertAsync(aux);
                            result = await FindAllAsync(filter);
                        }
                        response.Result = ResponseStatus.Ok;
                        response.Data = result[0].Movies;
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
            public async Task<ApiResponse<bool>> InsertLikedMovieAsync(LikedMovieModel model, UserModel loggeduser)
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

            public async Task <ApiResponse<bool>> MarkAsLikedAsync(MovieModel movie,UserModel loggeduser)
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

                    var filter = Builders<LikedMovieModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);

                    if(result.Count == 0)
                    {
                        LikedMovieModel aux = new LikedMovieModel()
                        {
                            Movies = new List<MovieModel>(),
                            UserId = loggeduser.Id
                        };
                        await InsertAsync(aux);
                        result = await FindAllAsync(filter);
                    }


                    result[0].Movies.Add(movie);
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

            public async Task<ApiResponse<bool>> UnMarkAsLikedAsync(MovieModel movie, UserModel loggeduser)
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

                    var filter = Builders<LikedMovieModel>.Filter.Eq(x => x.UserId, loggeduser.Id);
                    var result = await FindAllAsync(filter);

                    if (result.Count == 0)
                    {
                        LikedMovieModel aux = new LikedMovieModel()
                        {
                            Movies = new List<MovieModel>(),
                            UserId = loggeduser.Id
                        };
                        await InsertAsync(aux);
                        result = await FindAllAsync(filter);
                    }


                    result[0].Movies.RemoveAll(x => x.Title == movie.Title);
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

            public async Task<ApiResponse<bool>> UpdateLikedMovieAsync(LikedMovieModel model, UserModel loggeduser)
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
            public async Task<ApiResponse<bool>> DeleteLikedMovieAsync(LikedMovieModel model, UserModel loggeduser)
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
