using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;
using Peer.Domain.Collaborations;

namespace Peer.Infrastructure;
public class MongoDbCollaborationsRepository : ICollaborationsRepository
{
    private readonly IMongoCollection<Collaboration> _collaborationsCollection;

    public MongoDbCollaborationsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var collaborationsCollectionName = configuration["MongoDb:CollaborationsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _collaborationsCollection = mongoDb.GetCollection<Collaboration>(collaborationsCollectionName);
    }

    public Task AddAsync(Collaboration collaboration) =>
        _collaborationsCollection.InsertOneAsync(collaboration);
}
