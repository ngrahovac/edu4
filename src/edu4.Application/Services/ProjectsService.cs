using edu4.Application.Contracts;
using edu4.Application.Models;
using edu4.Domain.Projects;
using edu4.Domain.Users;
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

    public async Task<IReadOnlyList<Project>> GetRecommendedForUserWearing(Hat hat)
    {
        var projects = await _projects.GetRecommendedForUserWearing(hat);

        return projects;
    }

    public async Task<Project> PublishProjectAsync(
        string title,
        string description,
        Guid authorId,
        List<PositionDTO> positionData)
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
            positionData.Select(
                p => new Position(p.Name, p.Description, HatDTO.ToHat(p.Requirements))).ToList());

        await _projects.AddAsync(project);

        _logger.LogInformation("Project {Project} successfully published", project);

        return project;
    }

    public async Task<IReadOnlyList<Project>> DiscoverAsync(
        string? keyword = null,
        ProjectsSortOption sortOption = ProjectsSortOption.Default,
        Hat? usersHat = null)
    {
        var discoveredProjects = await _projects.DiscoverAsync(keyword, sortOption, usersHat);

        return discoveredProjects;
    }

    public async Task AddPositionAsync(
        Guid projectId,
        Guid requesterId,
        string positionName,
        string positionDescription,
        HatDTO positionRequirements)
    {
        var project = await _projects.GetByIdAsync(projectId) ??
            throw new InvalidOperationException("The project with the given Id does not exist");

        if (project.Author.Id != requesterId)
        {
            throw new InvalidOperationException("The requester doesn't have permission to update the project");
        }

        project.AddPosition(positionName, positionDescription, HatDTO.ToHat(positionRequirements));

        await _projects.UpdateAsync(project);
    }
}
