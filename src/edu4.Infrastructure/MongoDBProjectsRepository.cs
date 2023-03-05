using edu4.Application.Contracts;
using edu4.Domain.Projects;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace edu4.Infrastructure;
public class MongoDBProjectsRepository : IProjectsRepository
{
    private readonly IMongoCollection<Project> _projectsCollection;

    public MongoDBProjectsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var projectsCollectionName = configuration["MongoDb:ProjectsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _projectsCollection = mongoDb.GetCollection<Project>(projectsCollectionName);
    }

    public Task AddAsync(Project project)
        => _projectsCollection.InsertOneAsync(project);

    public Task<Project> GetByIdAsync(Guid id)
        => _projectsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
}
