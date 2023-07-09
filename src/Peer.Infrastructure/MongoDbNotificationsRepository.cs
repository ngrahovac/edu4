using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;
using Peer.Domain.Notifications;

namespace Peer.Infrastructure;
public class MongoDbNotificationsRepository : INotificationsRepository
{
    private readonly IMongoCollection<AbstractNotification> _notificationsCollection;

    public MongoDbNotificationsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var notificationsCollectionName = configuration["MongoDb:NotificationsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _notificationsCollection = mongoDb.GetCollection<AbstractNotification>(notificationsCollectionName);
    }
    public Task AddAsync(AbstractNotification notification) => _notificationsCollection.InsertOneAsync(notification);
}
