using edu4.Application;
using edu4.Domain.Users;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace edu4.Infrastructure;

public class MongoDbUsersRepository : IUsersRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public MongoDbUsersRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var usersCollectionName = configuration["MongoDb:UsersCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _usersCollection = mongoDb.GetCollection<User>(usersCollectionName);
    }

    public Task AddAsync(User user) =>
        _usersCollection.InsertOneAsync(user);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByAccountIdAsync(string accountId) =>
        await _usersCollection.Find(u => u.AccountId == accountId).FirstOrDefaultAsync();

}
