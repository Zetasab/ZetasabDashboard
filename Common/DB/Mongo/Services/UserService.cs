using MongoDB.Driver;
using RailwayDashboard.Common.DB.Mongo.DataModels;
using RailwayDashboard.Common.Models;
using static RailwayDashboard.Common.DB.Mongo.DataModels.MongoBase;

namespace RailwayDashboard.Common.DB.Mongo.Services
{
    public partial class BaseService : MongoRepositoryBase<User>
    {
        public BaseService(MongoContext context)
           : base(context, "user")
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
