using MongoDB.Driver;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public class UserService: MongoRepositoryBase<UserModel>
        {
            private List<string> thispage = new List<string>() { "zdb" };
            public UserService(MongoContext context)
                : base(context, "users") { }


            #region Get
            public async Task<ApiResponse<List<UserModel>>> GetAllUsersAsync(UserModel loggeduser)
            {
                ApiResponse<List<UserModel?>> response = new ApiResponse<List<UserModel?>>();
                try
                {
                    if (!HasPermissions(loggeduser, UserModel.EUserPermissionType.Visor, thispage))
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
                        response.Message = "Error obteniendo usuarios";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Error" + ex.Message;
                }
                return response;
            }
            #endregion

            #region Post
            public async Task<ApiResponse<UserModel?>> LoginAsync(UserModel user)
            {
                ApiResponse<UserModel?> response = new();

                try
                {
                    var filter = Builders<UserModel>.Filter.And(
                        Builders<UserModel>.Filter.Eq(u => u.Name, user.Name)
                    );

                    var first = await FindFirstAsync(filter);

                    if (first != null && BCrypt.Net.BCrypt.Verify(user.PasswordHash, first.PasswordHash))
                    {
                        response.Result = ResponseStatus.Ok;
                        response.Data = first;
                    }
                    else
                    {
                        response.Result = ResponseStatus.NotFound;
                        response.Message = "Email o contraseña incorrectos";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message += "Error:" + ex.Message;
                }

                return response;
            }
            public async Task<ApiResponse<bool>> InsertUserAsync(UserModel model, UserModel loggeduser)
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
                    response.Message = "El usuario se ha insertado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al insertar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            public async Task<ApiResponse<bool>> UpdateUserAsync(UserModel model, UserModel loggeduser)
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
                    response.Message = "El usuario se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al editar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeleteUserAsync(UserModel model, UserModel loggeduser)
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
                    response.Message = "El usuario se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = ResponseStatus.InternalError;
                    response.Message = "Ha ocurrido un error al borrar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }

    }
}
