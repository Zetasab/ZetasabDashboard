using MongoDB.Driver;
using ZetaDashboard.Common.Mongo;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService : MongoRepositoryBase<UserModel>
    {
        public BaseService(MongoContext context)
           : base(context, "users")
        {
        }

        #region Get
        public async Task<ApiResponse<UserModel>?> GetByEmailAsync(string email)
        {
            ApiResponse<UserModel?> response = new ApiResponse<UserModel?>();

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.Email, email);
                var first = await FindFirstAsync(filter);

                if (first != null)
                {
                    response.Result = true;
                    response.Data = first;
                }
                else
                {
                    response.Result = false;
                    response.Message = "No existe usuario";
                }
            }
            catch (Exception ex) 
            {
                response.Result = false;
                response.Message += "Error:" + ex.Message;
            }

            return response;
        }

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

        public async Task<ApiResponse<UserModel>?> LoginAsync(UserModel user)
        {
            ApiResponse<UserModel?> response = new ApiResponse<UserModel?>();

            try
            {
                var filter = Builders<UserModel>.Filter.And(
                    Builders<UserModel>.Filter.Eq(u => u.Email, user.Email),
                    Builders<UserModel>.Filter.Eq(u => u.PasswordHash, user.PasswordHash)
                );
                var first = await FindFirstAsync(filter);

                if (first != null)
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

        #endregion


        #region Insert
        public async Task AddUserAsync(UserModel user)
        {

            await InsertAsync(user);
        }
        #endregion
    }
}
