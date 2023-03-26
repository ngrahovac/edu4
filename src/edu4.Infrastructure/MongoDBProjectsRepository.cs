using edu4.Application.Contracts;
using edu4.Domain.Projects;
using edu4.Domain.Users;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
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


    public Task<Project> GetByIdAsync(Guid id)
        => _projectsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(Hat hat)
    {
        return hat switch
        {
            StudentHat sh => await GetRecommendedForUserWearing(sh),
            AcademicHat ah => await GetRecommendedForUserWearing(ah),
            _ => throw new NotImplementedException($"Getting projects recommended for a user with the hat {hat} is not implemented"),
        };
    }

    private async Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(StudentHat usersHat)
    {
        var equalOrHigherAcademicDegrees = Enum
            .GetValues<AcademicDegree>()
            .Where(d => d >= usersHat.AcademicDegree)
            .Select(d => $"{d}");

        var requirementsFilter = new BsonDocument
        {
            { $"{nameof(Position.Requirements)}._t", nameof(StudentHat) },
            { $"{nameof(Position.Requirements)}.{nameof(StudentHat.StudyField)}", usersHat.StudyField },
            { $"{nameof(Position.Requirements)}.{nameof(StudentHat.AcademicDegree)}", new BsonDocument {
                { "$in", new BsonArray(equalOrHigherAcademicDegrees) }
            }
            }
        };

        var stringFilterValue = requirementsFilter.ToString();

        var projectFilter = Builders<Project>.Filter.ElemMatch<Position>("_positions", requirementsFilter);

        var projects = await _projectsCollection.Find(projectFilter).ToListAsync();

        return projects;
    }

    private async Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(AcademicHat usersHat)
    {
        var requirementsFilter = new BsonDocument
        {
            { $"{nameof(Position.Requirements)}._t", nameof(AcademicHat) },
            { $"{nameof(Position.Requirements)}.{nameof(AcademicHat.ResearchField)}", usersHat.ResearchField }
        };

        var projectFilter = Builders<Project>.Filter.ElemMatch<Position>("_positions", requirementsFilter);

        var projects = await _projectsCollection.Find(projectFilter).ToListAsync();

        return projects;
    }

    public async Task<IReadOnlyList<Project>> DiscoverAsync(string keyword, ProjectsSortOption sortOption)
    {
        var keywordInProjectTitleFilter = Builders<Project>.Filter.Where(p => p.Title.Contains(keyword));
        var keywordInProjectDescriptionFilter = Builders<Project>.Filter.Where(p => p.Description.Contains(keyword));
        var keywordInPositionNameFilter = Builders<Position>.Filter.Where(p => p.Name.Contains(keyword));
        var keywordInPositionDescriptionFilter = Builders<Position>.Filter.Where(p => p.Description.Contains(keyword));
        var keywordInAnyPositionNameOrDescriptionFilter = Builders<Project>.Filter.ElemMatch("_positions", Builders<Position>.Filter.Or(
            keywordInPositionNameFilter, keywordInPositionDescriptionFilter));

        var projectsFilter = Builders<Project>.Filter
            .Or(keywordInProjectTitleFilter,
            keywordInProjectDescriptionFilter,
            keywordInAnyPositionNameOrDescriptionFilter);

        var sorting = sortOption switch
        {
            ProjectsSortOption.OldestFirst => Builders<Project>.Sort.Ascending(p => p.DatePosted),
            ProjectsSortOption.NewestFirst => Builders<Project>.Sort.Descending(p => p.DatePosted),
            ProjectsSortOption.Default => null,
            _ => throw new NotImplementedException()
        };

        var projects = sorting is not null ?
            await _projectsCollection.Find(projectsFilter).Sort(sorting).ToListAsync() :
            await _projectsCollection.Find(projectsFilter).ToListAsync();

        return projects;
    }

    public Task AddAsync(Project project)
        => _projectsCollection.InsertOneAsync(project);
}
