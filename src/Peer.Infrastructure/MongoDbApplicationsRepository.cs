using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;

namespace Peer.Infrastructure;
public class MongoDbApplicationsRepository : IApplicationsRepository
{
    private readonly IMongoCollection<Domain.Applications.Application> _applicationsCollection;

    public MongoDbApplicationsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var applicationsCollectionName = configuration["MongoDb:ApplicationsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _applicationsCollection = mongoDb.GetCollection<Domain.Applications.Application>(applicationsCollectionName);
    }

    public Task<Domain.Applications.Application> GetByApplicantAndPositionAsync(Guid applicantId, Guid positionId) =>
        _applicationsCollection.Find(a => a.ApplicantId == applicantId && a.PositionId == positionId).FirstOrDefaultAsync();

    public Task AddAsync(Domain.Applications.Application application) =>
        _applicationsCollection.InsertOneAsync(application);
}
