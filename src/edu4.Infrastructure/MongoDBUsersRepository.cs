using edu4.Application.Contracts;
using edu4.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace edu4.Infrastructure;

public class MongoDbUsersRepository : IUsersRepository
{
    private readonly IMongoCollection<Contributor> _usersCollection;

    public MongoDbUsersRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var usersCollectionName = configuration["MongoDb:UsersCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _usersCollection = mongoDb.GetCollection<Contributor>(usersCollectionName);
    }

    public Task AddAsync(Contributor user)
        => _usersCollection.InsertOneAsync(user);

    public async Task<Contributor?> GetByIdAsync(Guid id)
        => await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<Contributor?> GetByAccountIdAsync(string accountId)
        => await _usersCollection.Find(u => u.AccountId == accountId).FirstOrDefaultAsync();

}
