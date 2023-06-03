using Peer.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;

namespace Peer.Infrastructure;

public class MongoDbContributorsRepository : IContributorsRepository
{
    private readonly IMongoCollection<Contributor> _contributorsCollection;
    private readonly FilterDefinition<Contributor> _nonRemovedContributorFilter =
        Builders<Contributor>.Filter.Where(c => !c.Removed);

    public MongoDbContributorsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var usersCollectionName = configuration["MongoDb:UsersCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _contributorsCollection = mongoDb.GetCollection<Contributor>(usersCollectionName);
    }

    private Task<Contributor> FindOneAsync(FilterDefinition<Contributor> filter) =>
        _contributorsCollection
        .Find(Builders<Contributor>.Filter.And(filter, _nonRemovedContributorFilter))
        .SingleOrDefaultAsync();

    public Task AddAsync(Contributor user)
        => _contributorsCollection.InsertOneAsync(user);

    public Task<Contributor> GetByIdAsync(Guid id)
        => FindOneAsync(Builders<Contributor>.Filter.Where(c => c.Id == id));

    public Task<Contributor> GetByAccountIdAsync(string accountId)
        => FindOneAsync(Builders<Contributor>.Filter.Where(c => c.AccountId == accountId));

    public async Task UpdateAsync(Contributor contributor)
    {
        var updateFilter = Builders<Contributor>.Update
            .Set(p => p.FullName, contributor.FullName)
            .Set(p => p.ContactEmail, contributor.ContactEmail)
            .Set(p => p.Hats, contributor.Hats)
            .Set(c => c.Removed, contributor.Removed);

        await _contributorsCollection.UpdateOneAsync(c => c.Id == contributor.Id, updateFilter);
    }

    public Task RemoveAsync(Contributor contributor) =>
        _contributorsCollection.DeleteOneAsync(c => c.Id == contributor.Id);
}
