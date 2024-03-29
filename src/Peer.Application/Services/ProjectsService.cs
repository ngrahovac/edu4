using Peer.Domain.Projects;
using Microsoft.Extensions.Logging;
using Peer.Application.Contracts;
using Peer.Application.Models;
using Peer.Domain.Contributors;

namespace Peer.Application.Services;

public class ProjectsService
{
    private readonly IProjectsRepository _projects;
    private readonly IContributorsRepository _users;
    private readonly IDomainEventsRepository _domainEvents;
    private readonly ILogger<ProjectsService> _logger;

    public ProjectsService(
        IProjectsRepository projects,
        IContributorsRepository users,
        IDomainEventsRepository domainEvents,
        ILogger<ProjectsService> logger
    )
    {
        _projects = projects;
        _users = users;
        _domainEvents = domainEvents;
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
        DateTime datePosted,
        DateTime startDate,
        DateTime? endDate,
        List<PositionDTO> positionData
    )
    {
        var author = await _users.GetByIdAsync(authorId);

        if (author is null)
        {
            _logger.LogError(
                "Error publishing project: author with id {AuthorId} does not exist",
                authorId
            );
            throw new NotImplementedException(
                $"Error publishing project: author with id {authorId} does not exist"
            );
        }

        var project = new Project(
            title,
            description,
            authorId,
            datePosted,
            startDate,
            endDate,
            positionData
                .Select(p => new Position(p.Name, p.Description, HatDTO.ToHat(p.Requirements)))
                .ToList()
        );

        await _projects.AddAsync(project);

        _logger.LogInformation("Project {Project} successfully published", project);

        return project;
    }

    public async Task<PagedList<Project>> DiscoverAsync(
        Guid requesterId,
        string? keyword = null,
        ProjectsSortOption sortOption = ProjectsSortOption.Unspecified,
        Hat? usersHat = null,
        int page = 1,
        int pageSize = 5)
    {
        var discoveredProjects = await _projects.DiscoverAsync(requesterId, keyword, sortOption, usersHat, page, pageSize);

        return discoveredProjects;
    }

    public async Task<Project> GetByIdAsync(Guid projectId)
    {
        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given Id does not exist");

        return project;
    }

    public async Task<List<Project>> GetByAuthorIdAsync(Guid authorId)
    {
        var projects = await _projects.GetByAuthorAsync(authorId);

        return projects;
    }

    public async Task AddPositionAsync(
        Guid projectId,
        Guid requesterId,
        string positionName,
        string positionDescription,
        HatDTO positionRequirements
    )
    {
        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given Id does not exist");

        if (project.AuthorId != requesterId)
        {
            throw new InvalidOperationException(
                "The requester doesn't have permission to update the project"
            );
        }

        project.AddPosition(positionName, positionDescription, HatDTO.ToHat(positionRequirements));

        await _projects.UpdateAsync(project);
    }

    public async Task UpdateDetailsAsync(
        Guid projectId,
        Guid requesterId,
        string title,
        string description
    )
    {
        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given Id does not exist");

        if (project.AuthorId != requesterId)
        {
            throw new InvalidCastException(
                "The requester doesn't have permission to update the project"
            );
        }

        project.UpdateDetails(title, description);

        await _projects.UpdateAsync(project);
    }

    public async Task RemoveAsync(Guid projectId, Guid requesterId)
    {
        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given Id does not exist");

        if (project.AuthorId != requesterId)
        {
            throw new InvalidOperationException(
                "The requester doesn't have permission to update the project"
            );
        }

        project.Remove();

        // TODO: wrap in a transaction
        await _projects.UpdateAsync(project);
        await Task.WhenAll(project.DomainEvents.Select(de => _domainEvents.AddAsync(de)));
    }

    public async Task ClosePositionAsync(Guid requesterId, Guid projectId, Guid positionId)
    {
        var requester =
            await _users.GetByIdAsync(requesterId)
            ?? throw new InvalidOperationException(
                "The contributor with the given id doesn't exist"
            );

        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given id doesn't exist");

        if (requester!.Id != project.AuthorId)
        {
            throw new InvalidOperationException(
                "Only the project author can close a project position"
            );
        }

        project.ClosePosition(positionId);

        // TODO: wrap in a transaction
        await _projects.UpdateAsync(project);
        await Task.WhenAll(project.DomainEvents.Select(de => _domainEvents.AddAsync(de)));
    }

    public async Task ReopenPositionAsync(Guid requesterId, Guid projectId, Guid positionId)
    {
        var requester =
            await _users.GetByIdAsync(requesterId)
            ?? throw new InvalidOperationException(
                "The contributor with the given id doesn't exist"
            );

        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given id doesn't exist");

        if (requester!.Id != project.AuthorId)
        {
            throw new InvalidOperationException(
                "Only the project author can reopen a project position"
            );
        }

        project.ReopenPosition(positionId);

        await _projects.UpdateAsync(project);
    }

    public async Task RemovePositionAsync(Guid requesterId, Guid projectId, Guid positionId)
    {
        var requester =
            await _users.GetByIdAsync(requesterId)
            ?? throw new InvalidOperationException(
                "The contributor with the given id doesn't exist"
            );

        var project =
            await _projects.GetByIdAsync(projectId)
            ?? throw new InvalidOperationException("The project with the given id doesn't exist");

        if (requester!.Id != project.AuthorId)
        {
            throw new InvalidOperationException(
                "Only the project author can remove a project position"
            );
        }

        project.RemovePosition(positionId);

        // TODO: wrap in a transaction
        await _projects.UpdateAsync(project);
        await Task.WhenAll(project.DomainEvents.Select(de => _domainEvents.AddAsync(de)));
    }
}
