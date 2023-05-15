using Peer.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;

namespace Peer.Infrastructure;

public class MongoDbContributorsRepository : IContributorsRepository
{
    private readonly IMongoCollection<Contributor> _usersCollection;

    public MongoDbContributorsRepository(IConfiguration configuration)
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
