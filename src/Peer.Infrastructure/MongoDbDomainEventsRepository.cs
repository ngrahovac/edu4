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
}
