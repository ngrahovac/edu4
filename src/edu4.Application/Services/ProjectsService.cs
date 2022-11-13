using edu4.Application.Contracts;
using edu4.Domain.Projects;
using Microsoft.Extensions.Logging;

namespace edu4.Application.Services;
public class ProjectsService
{
    private readonly IProjectsRepository _projects;
    private readonly IUsersRepository _users;
    private readonly ILogger<ProjectsService> _logger;

    public ProjectsService(
        IProjectsRepository projects,
        IUsersRepository users,
        ILogger<ProjectsService> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task<Project> PublishProjectAsync(
        string title,
        string description,
        Guid authorId,
        List<PositionDTO> positions)
    {
        var author = await _users.GetByIdAsync(authorId);

        if (author is null)
        {
            _logger.LogError("Error publishing project: author with id {AuthorId} does not exist", authorId);
            throw new NotImplementedException($"Error publishing project: author with id {authorId} does not exist");
        }

        var project = new Project(
            title,
            description,
            new Author(authorId),
            positions.Select(
                p => new Position(p.Name, p.Description, HatDTO.ToHat(p.Requirements))).ToList());

        await _projects.AddAsync(project);

        _logger.LogInformation("Project {Project} successfully published", project);

        return project;
    }
}
