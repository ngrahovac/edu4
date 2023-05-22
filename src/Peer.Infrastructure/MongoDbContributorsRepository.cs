using Peer.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;

namespace Peer.Infrastructure;

public class MongoDbContributorsRepository : IContributorsRepository
{
    private readonly IMongoCollection<Contributor> _contributorsCollection;

    public MongoDbContributorsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var usersCollectionName = configuration["MongoDb:UsersCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _contributorsCollection = mongoDb.GetCollection<Contributor>(usersCollectionName);
    }

    public Task AddAsync(Contributor user)
        => _contributorsCollection.InsertOneAsync(user);

    public async Task<Contributor?> GetByIdAsync(Guid id)
        => await _contributorsCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<Contributor?> GetByAccountIdAsync(string accountId)
        => await _contributorsCollection.Find(u => u.AccountId == accountId).FirstOrDefaultAsync();

    public async Task UpdateAsync(Contributor contributor)
    {
        var updateFilter = Builders<Contributor>.Update
            .Set(p => p.FullName, contributor.FullName)
            .Set(p => p.ContactEmail, contributor.ContactEmail)
            .Set(p => p.Hats, contributor.Hats);

        await _contributorsCollection.UpdateOneAsync(c => c.Id == contributor.Id, updateFilter);
    }
}
