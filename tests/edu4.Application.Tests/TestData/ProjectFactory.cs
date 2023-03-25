using edu4.Application.Contracts;
using edu4.Domain.Projects;
using edu4.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace edu4.Application.Tests.TestData;

internal class ProjectFactory
{
    private string _title = "Test project title";
    private string _description = "Test project description";
    private Guid _authorId = Guid.NewGuid();
    private List<Position> _positions = new();
    private readonly IProjectsRepository _projects;

    public ProjectFactory()
    {
        var config = new ConfigurationBuilder().AddUserSecrets(typeof(ProjectFactory).Assembly)
            .Build();

        _projects = new MongoDBProjectsRepository(config);
    }

    public ProjectFactory WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ProjectFactory WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProjectFactory WithAuthorId(Guid authorId)
    {
        _authorId = authorId;
        return this;
    }

    public ProjectFactory WithPositions(List<Position> positions)
    {
        _positions = positions;
        return this;
    }

    public async Task<Project> SeedAsync()
    {
        var project = new Project(
            _title,
            _description,
            new Author(_authorId),
            _positions);

        await _projects.AddAsync(project);

        return project;
    }
}
