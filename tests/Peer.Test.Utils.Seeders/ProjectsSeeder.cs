using Microsoft.Extensions.Configuration;
using Peer.Application.Contracts;
using Peer.Domain.Projects;
using Peer.Infrastructure;
using Peer.Tests.Utils.Factories;

namespace Peer.Test.Utils.Seeders;

public class ProjectsSeeder
{
    private readonly IProjectsRepository _projects;
    private readonly ProjectsFactory _projectsFactory;

    public ProjectsSeeder(IConfiguration configuration)
    {
        _projects = new MongoDBProjectsRepository(configuration);
        _projectsFactory = new ProjectsFactory();
    }

    public ProjectsSeeder WithTitle(string title)
    {
       _projectsFactory.WithTitle(title);
        return this;
    }

    public ProjectsSeeder WithDescription(string description)
    {
        _projectsFactory.WithDescription(description);
        return this;
    }

    public ProjectsSeeder WithAuthorId(Guid authorId)
    {
        _projectsFactory.WithAuthorId(authorId);
        return this;
    }

    public ProjectsSeeder WithPositions(List<Position> positions)
    {
        _projectsFactory.WithPositions(positions);
        return this;
    }

    public async Task<Project> SeedAsync()
    {
        var project = _projectsFactory.Build();

        await _projects.AddAsync(project);

        return project;
    }
}
