using MongoDB.Driver;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService : MongoRepositoryBase<User>
    {
        public BaseService(MongoContext context)
           : base(context, "users")
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await FindFirstAsync(filter);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await FindAllAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await InsertAsync(user);
        }
    }
}
