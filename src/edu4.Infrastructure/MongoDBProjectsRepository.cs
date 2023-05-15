using Peer.Application.Contracts;
using Peer.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Peer.Domain.Projects;

namespace Peer.Infrastructure;
public class MongoDBProjectsRepository : IProjectsRepository
{
    private readonly IMongoCollection<Project> _projectsCollection;
    private readonly FilterDefinition<Project> _emptyProjectFilter =
        Builders<Project>.Filter.Empty;

    private readonly FilterDefinition<Position> _emptyPositionFilter =
        Builders<Position>.Filter.Empty;

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

    public async Task<IReadOnlyList<Project>> DiscoverAsync(string? keyword, ProjectsSortOption sortOption, Hat? usersHat)
    {
        var keywordInProjectTitleFilter =
            keyword is not null ?
            Builders<Project>.Filter.Where(p => p.Title.Contains(keyword)) :
            _emptyProjectFilter;

        var keywordInProjectDescriptionFilter =
            keyword is not null ?
            Builders<Project>.Filter.Where(p => p.Description.Contains(keyword)) :
            _emptyProjectFilter;

        var keywordInPositionNameFilter =
            keyword is not null ?
            Builders<Position>.Filter.Where(p => p.Name.Contains(keyword)) :
            _emptyPositionFilter;

        var keywordInPositionDescriptionFilter =
            keyword is not null ?
            Builders<Position>.Filter.Where(p => p.Description.Contains(keyword)) :
            _emptyPositionFilter;

        var keywordInAnyPositionNameOrDescriptionFilter = Builders<Project>.Filter.ElemMatch("_positions", Builders<Position>.Filter.Or(
            keywordInPositionNameFilter, keywordInPositionDescriptionFilter));

        var requirementsFilter = usersHat is not null ?
            GetFilterForProjectsFitForAUserWearingTheHat(usersHat) :
            _emptyProjectFilter;

        var filter = Builders<Project>.Filter
            .And(requirementsFilter,
            Builders<Project>.Filter.Or(keywordInProjectTitleFilter,
            keywordInProjectDescriptionFilter,
            keywordInAnyPositionNameOrDescriptionFilter));

        var sorting = sortOption switch
        {
            ProjectsSortOption.Asc => Builders<Project>.Sort.Ascending(p => p.DatePosted),
            ProjectsSortOption.Desc => Builders<Project>.Sort.Descending(p => p.DatePosted),
            ProjectsSortOption.Default => null,
            _ => throw new NotImplementedException()
        };

        var projects = sorting is not null ?
            await _projectsCollection.Find(filter).Sort(sorting).ToListAsync() :
            await _projectsCollection.Find(filter).ToListAsync();

        return projects;
    }

    private FilterDefinition<Project> GetFilterForProjectsFitForAUserWearingTheHat(Hat usersHat)
    {
        if (usersHat is StudentHat studentHat)
        {
            return GetFilterForProjectsFitForAUserWearingTheStudentHat(studentHat);
        }
        else if (usersHat is AcademicHat academicHat)
        {
            return GetFilterForProjectsFitForAUserWearingTheAcademicHat(academicHat);
        }

        throw new NotImplementedException("Getting recommended projects for hat type not yet supported");
    }

    private FilterDefinition<Project> GetFilterForProjectsFitForAUserWearingTheStudentHat(StudentHat usersHat)
    {
        var requiredEqualOrLowerAcademicDegree = Enum
            .GetValues<AcademicDegree>()
            .Where(d => d <= usersHat.AcademicDegree)
            .Select(d => $"{d}");

        var requirementsFilter = new BsonDocument
        {
            { $"{nameof(Position.Requirements)}._t", nameof(StudentHat) },
            { $"{nameof(Position.Requirements)}.{nameof(StudentHat.StudyField)}", usersHat.StudyField },
            { $"{nameof(Position.Requirements)}.{nameof(StudentHat.AcademicDegree)}", new BsonDocument {
                { "$in", new BsonArray(requiredEqualOrLowerAcademicDegree) }
            }
            }
        };

        var stringFilterValue = requirementsFilter.ToString();

        var projectFilter = Builders<Project>.Filter.ElemMatch<Position>("_positions", requirementsFilter);
        return projectFilter;
    }

    private FilterDefinition<Project> GetFilterForProjectsFitForAUserWearingTheAcademicHat(AcademicHat usersHat)
    {
        var requirementsFilter = new BsonDocument
        {
            { $"{nameof(Position.Requirements)}._t", nameof(AcademicHat) },
            { $"{nameof(Position.Requirements)}.{nameof(AcademicHat.ResearchField)}", usersHat.ResearchField }
        };

        var projectFilter = Builders<Project>.Filter.ElemMatch<Position>("_positions", requirementsFilter);

        return projectFilter;
    }

    public Task AddAsync(Project project)
        => _projectsCollection.InsertOneAsync(project);

    public async Task UpdateAsync(Project project)
    {
        var updateFilter = Builders<Project>.Update
            .Set(p => p.Title, project.Title)
            .Set(p => p.Description, project.Description)
            .Set(p => p.Positions, project.Positions);

        await _projectsCollection.UpdateOneAsync(p => p.Id == project.Id, updateFilter);
    }

    public async Task DeleteAsync(Guid projectId)
    {
        await _projectsCollection.DeleteOneAsync(p => p.Id == projectId);
    }
}
