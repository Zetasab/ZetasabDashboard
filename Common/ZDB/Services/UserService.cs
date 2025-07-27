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
            public UserService(MongoContext context)
                : base(context, "users") { }


            #region Get
            public async Task<ApiResponse<List<UserModel>>> GetAllUsersAsync()
            {
                ApiResponse<List<UserModel?>> response = new ApiResponse<List<UserModel?>>();
                try
                {
                    var result = await FindAllAsync();
                    if (result != null)
                    {
                        response.Result = true;
                        response.Data = result;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "Error obteniendo usuarios";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = false;
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
                        response.Result = true;
                        response.Data = first;
                    }
                    else
                    {
                        response.Result = false;
                        response.Message = "Email o contraseña incorrectos";
                    }
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message += "Error:" + ex.Message;
                }

                return response;
            }
            public async Task<ApiResponse<bool>> InsertUserAsync(UserModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await InsertAsync(model);
                    response.Result = true;
                    response.Message = "El usuario se ha insertado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al insertar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Update
            public async Task<ApiResponse<bool>> UpdateUserAsync(UserModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await UpdateAsync(model);
                    response.Result = true;
                    response.Message = "El usuario se ha editado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al editar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion

            #region Delete
            public async Task<ApiResponse<bool>> DeleteUserAsync(UserModel model)
            {
                ApiResponse<bool> response = new ApiResponse<bool>();

                try
                {
                    await DeleteAsync(model);
                    response.Result = true;
                    response.Message = "El usuario se ha borrado correctamente";
                }
                catch (Exception ex)
                {
                    response.Result = false;
                    response.Message = "Ha ocurrido un error al borrar el usuario";
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return response;
            }
            #endregion
        }

    }
}
