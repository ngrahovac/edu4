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

    public Task<Domain.Applications.Application> GetByIdAsync(Guid id) =>
        _applicationsCollection.Find(a => a.Id == id).SingleOrDefaultAsync();

    public Task UpdateAsync(Domain.Applications.Application application)
    {
        var update = Builders<Domain.Applications.Application>.Update
            .Set(a => a.Status, application.Status);

        return _applicationsCollection.UpdateOneAsync(a => a.Id == application.Id, update);
    }
}
