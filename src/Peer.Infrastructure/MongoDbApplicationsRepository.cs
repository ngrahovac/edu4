using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Peer.Application.Contracts;
using Peer.Domain.Applications;
using Peer.Domain.Projects;

namespace Peer.Infrastructure;
public class MongoDbApplicationsRepository : IApplicationsRepository
{
    private readonly IMongoCollection<Domain.Applications.Application> _applicationsCollection;
    private readonly IMongoCollection<Project> _projectsCollection;

    public MongoDbApplicationsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var applicationsCollectionName = configuration["MongoDb:ApplicationsCollectionName"];
        var projectsCollectionName = configuration["MongoDb:ProjectsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _applicationsCollection = mongoDb.GetCollection<Domain.Applications.Application>(applicationsCollectionName);
        _projectsCollection = mongoDb.GetCollection<Project>(projectsCollectionName);
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

    public async Task<List<Domain.Applications.Application>> GetReceivedAsync(
        Guid requesterId,
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption applicationsSortOption)
    {
        var projectFilter = Builders<Domain.Applications.Application>.Filter.Empty;
        var positionFilter = Builders<Domain.Applications.Application>.Filter.Empty;
        var submittedApplicationsFilter = Builders<Domain.Applications.Application>.Filter
            .Where(a => a.Status == ApplicationStatus.Submitted);

        var sorting = applicationsSortOption switch
        {
            ApplicationsSortOption.Default => null,
            ApplicationsSortOption.NewestFirst => Builders<Domain.Applications.Application>.Sort.Descending(a => a.DateSubmitted),
            ApplicationsSortOption.OldestFirst => Builders<Domain.Applications.Application>.Sort.Ascending(a => a.DateSubmitted),
            _ => throw new NotImplementedException()
        };

        // if projectId isn't specified, query incoming applications for all authored projects
        if (projectId is null)
        {
            // TODO: do a lookup here?
            var authoredProjectIds = await _projectsCollection
                .Find(p => p.AuthorId == requesterId && !p.Removed)
                .Project(p => p.Id)
                .ToListAsync();

            projectFilter = Builders<Domain.Applications.Application>.Filter
                .Where(a => authoredProjectIds.Contains(a.ProjectId));

            var filter = Builders<Domain.Applications.Application>.Filter
                .And(projectFilter, submittedApplicationsFilter);

            return await _applicationsCollection.Find(filter).Sort(sorting).ToListAsync();
        }
        else
        {
            projectFilter = Builders<Domain.Applications.Application>.Filter
                .Where(a => a.ProjectId == projectId);

            if (positionId is not null)
            {
                positionFilter = Builders<Domain.Applications.Application>.Filter
                    .Where(a => a.PositionId == positionId);
            }

            var filter = Builders<Domain.Applications.Application>.Filter
                .And(projectFilter, positionFilter, submittedApplicationsFilter);

            return await _applicationsCollection.Find(filter).Sort(sorting).ToListAsync();
        }
    }

    public async Task<List<Domain.Applications.Application>> GetSentAsync(
        Guid requesterId,
        Guid? projectId,
        Guid? positionId,
        ApplicationsSortOption applicationsSortOption)
    {
        var projectFilter = projectId is null ?
            Builders<Domain.Applications.Application>.Filter.Empty :
            Builders<Domain.Applications.Application>.Filter.Where(a => a.ProjectId == projectId);

        var positionFilter = positionId is null ?
            Builders<Domain.Applications.Application>.Filter.Empty :
            Builders<Domain.Applications.Application>.Filter.Where(a => a.PositionId == positionId);

        var submittedApplicationsFilter = Builders<Domain.Applications.Application>.Filter
            .Where(a => a.Status == ApplicationStatus.Submitted);

        var sorting = applicationsSortOption switch
        {
            ApplicationsSortOption.Default => null,
            ApplicationsSortOption.NewestFirst => Builders<Domain.Applications.Application>.Sort.Descending(a => a.DateSubmitted),
            ApplicationsSortOption.OldestFirst => Builders<Domain.Applications.Application>.Sort.Ascending(a => a.DateSubmitted),
            _ => throw new NotImplementedException()
        };

        var filter = Builders<Domain.Applications.Application>.Filter
            .And(projectFilter, positionFilter, submittedApplicationsFilter);

        return await _applicationsCollection.Find(filter).Sort(sorting).ToListAsync();
    }

    public Task<List<Domain.Applications.Application>> GetByApplicantAsync(Guid applicantId)
    {
        var applicantFilter = Builders<Domain.Applications.Application>.Filter.Where(a => a.ApplicantId == applicantId);

        var submittedApplicationsFilter = Builders<Domain.Applications.Application>.Filter
            .Where(a => a.Status == ApplicationStatus.Submitted);

        var filter = Builders<Domain.Applications.Application>.Filter
            .And(applicantFilter, submittedApplicationsFilter);

        return _applicationsCollection.Find(filter).ToListAsync();
    }

}
