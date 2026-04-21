using mongo_api.Model;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace mongo_api.Services
{
    public class mongoService
    {
        private readonly IMongoCollection<User> _users;

        public mongoService(IConfiguration config)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("mydb");
            _users = db.GetCollection<User>("users");
        }

        public IMongoCollection<User> Users => _users;
    }
}
