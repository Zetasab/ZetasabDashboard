using MongoDB.Driver;
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

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq(u => u.Email, email);
            return await FindFirstAsync(filter);
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await FindAllAsync();
        }

        public async Task AddUserAsync(UserModel user)
        {
            await InsertAsync(user);
        }
    }
}
