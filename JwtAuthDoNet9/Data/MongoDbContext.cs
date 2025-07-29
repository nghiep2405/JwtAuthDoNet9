using MongoDB.Driver;
using ReactForUI.Server.Entities;

namespace ReactForUI.Server.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDbSettings:ConnectionString"];
            var databaseName = configuration["MongoDbSettings:DatabaseName"];
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Book> Books => _database.GetCollection<Book>("books");
        public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("carts");
    }
}