using Peer.Application.Contracts;
using Peer.Domain.Contributors;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Peer.Domain.Projects;

namespace Peer.Infrastructure;
public class MongoDBProjectsRepository : IProjectsRepository
{
    private readonly int _pageSize = 3;

    private readonly IMongoCollection<Project> _projectsCollection;
    private readonly FilterDefinition<Project> _emptyProjectFilter =
        Builders<Project>.Filter.Empty;

    private readonly FilterDefinition<Position> _emptyPositionFilter =
        Builders<Position>.Filter.Empty;

    private readonly FilterDefinition<Project> _nonRemovedProjectsFilter =
        Builders<Project>.Filter.Where(p => !p.Removed);

    public MongoDBProjectsRepository(IConfiguration configuration)
    {
        var clusterConnectionString = configuration["MongoDb:ClusterConnectionString"];
        var dbName = configuration["MongoDb:DbName"];
        var projectsCollectionName = configuration["MongoDb:ProjectsCollectionName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);
        _projectsCollection = mongoDb.GetCollection<Project>(projectsCollectionName);
    }

    private Task<Project> FindOneAsync(FilterDefinition<Project> filter) =>
        _projectsCollection
        .Find(Builders<Project>.Filter.And(filter, _nonRemovedProjectsFilter))
        .SingleOrDefaultAsync();

    private Task<List<Project>> FindManyAsync(FilterDefinition<Project> filter, SortDefinition<Project>? sort = null, int page = 1) =>
        _projectsCollection
        .Find(Builders<Project>.Filter.And(filter, _nonRemovedProjectsFilter))
        .Sort(sort)
        .Skip((page - 1) * _pageSize)
        .Limit(_pageSize)
        .ToListAsync();

    private Task<long> CountManyAsync(FilterDefinition<Project> filter) =>
            _projectsCollection
            .CountDocumentsAsync(Builders<Project>.Filter.And(filter, _nonRemovedProjectsFilter));

    public Task<Project> GetByIdAsync(Guid id)
        => FindOneAsync(Builders<Project>.Filter.Where(p => p.Id == id));

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

        var projects = await FindManyAsync(projectFilter);

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

        var projects = await FindManyAsync(projectFilter);

        return projects;
    }

    public async Task<IReadOnlyList<Project>> DiscoverAsync(Guid requesterId, string? keyword, ProjectsSortOption sortOption, Hat? usersHat, int page = 1)
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

        var nonAuthoredFilter = Builders<Project>.Filter.Where(p => p.AuthorId != requesterId);

        var filter = Builders<Project>.Filter
            .And(
            requirementsFilter,
            nonAuthoredFilter,
            Builders<Project>.Filter.Or(keywordInProjectTitleFilter,
            keywordInProjectDescriptionFilter,
            keywordInAnyPositionNameOrDescriptionFilter));

        var sorting = sortOption switch
        {
            ProjectsSortOption.ByDatePostedAsc => Builders<Project>.Sort.Ascending(p => p.DatePosted),
            ProjectsSortOption.ByDatePostedDesc => Builders<Project>.Sort.Descending(p => p.DatePosted),
            ProjectsSortOption.Unspecified => null,
            _ => throw new NotImplementedException()
        };

        var totalDiscovered = await CountManyAsync(filter);
        var projects = await FindManyAsync(filter, sorting, page);

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
            .Set(p => p.Positions, project.Positions)
            .Set(p => p.Removed, project.Removed);

        await _projectsCollection.UpdateOneAsync(p => p.Id == project.Id, updateFilter);
    }

    public Task<List<Project>> GetByAuthorAsync(Guid authorId)
    {
        var authorFilter = Builders<Project>.Filter
            .Where(p => p.AuthorId == authorId);

        return FindManyAsync(authorFilter);
    }
}
