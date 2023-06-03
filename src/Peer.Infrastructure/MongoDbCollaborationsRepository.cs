using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;
using Peer.Domain.Collaborations;

namespace Peer.Infrastructure;
public class MongoDbCollaborationsRepository : ICollaborationsRepository
{
    private readonly IMongoCollection<Collaboration> _collaborationsCollection;
    private readonly FilterDefinition<Collaboration> _activeCollaborationsFilter =
        Builders<Collaboration>.Filter.Where(c => c.Status == CollaborationStatus.Active);

    public MongoDbCollaborationsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var collaborationsCollectionName = configuration["MongoDb:CollaborationsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _collaborationsCollection = mongoDb.GetCollection<Collaboration>(collaborationsCollectionName);
    }

    private Task<Collaboration> FindOneAsync(FilterDefinition<Collaboration> filter) =>
        _collaborationsCollection
        .Find(Builders<Collaboration>.Filter.And(filter, _activeCollaborationsFilter))
        .SingleOrDefaultAsync();


    public Task AddAsync(Collaboration collaboration) =>
        _collaborationsCollection.InsertOneAsync(collaboration);

    public Task<Collaboration> GetByIdAsync(Guid collaborationId) =>
        FindOneAsync(Builders<Collaboration>.Filter.Where(c => c.Id == collaborationId));

    public async Task UpdateAsync(Collaboration collaboration)
    {
        var update = Builders<Collaboration>.Update
            .Set(c => c.Status, collaboration.Status);

        await _collaborationsCollection.UpdateOneAsync(c => c.Id == collaboration.Id, update);
    }
}
