using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;
using Peer.Domain.Common;

namespace Peer.Infrastructure;
public class MongoDbDomainEventsRepository : IDomainEventsRepository
{
    private readonly IMongoCollection<AbstractDomainEvent> _domainEventsCollection;

    public MongoDbDomainEventsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var domainEventsCollectionName = configuration["MongoDb:DomainEventsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _domainEventsCollection = mongoDb.GetCollection<AbstractDomainEvent>(domainEventsCollectionName);
    }
    public Task AddAsync(AbstractDomainEvent domainEvent) => _domainEventsCollection.InsertOneAsync(domainEvent);
    public Task<List<AbstractDomainEvent>> GetUnprocessedBatchAsync(int batchSize)
    {
        var unprocessedFilter = Builders<AbstractDomainEvent>.Filter
            .Where(de => !de.Processed);

        return _domainEventsCollection.Find(unprocessedFilter).Limit(batchSize).ToListAsync();
    }

    public Task UpdateAsync(AbstractDomainEvent domainEvent)
    {
        var update = Builders<AbstractDomainEvent>.Update
            .Set(de => de.Processed, domainEvent.Processed);

        return _domainEventsCollection.UpdateOneAsync(de => de.Id == domainEvent.Id, update);
    }
}
