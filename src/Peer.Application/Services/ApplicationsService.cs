using Microsoft.Extensions.Logging;
using Peer.Application.Contracts;
using Peer.Domain.Applications;

namespace Peer.Application.Services;
public class ApplicationsService
{
    private readonly IApplicationsRepository _applications;
    private readonly IContributorsRepository _contributors;
    private readonly IProjectsRepository _projects;
    private readonly IDomainEventsRepository _domainEvents;
    private readonly ILogger<ApplicationsService> _logger;

    public ApplicationsService(
        IApplicationsRepository applications,
        IContributorsRepository contributors,
        IProjectsRepository projects,
        IDomainEventsRepository domainEvents,
        ILogger<ApplicationsService> logger)
    {
        _applications = applications;
        _contributors = contributors;
        _projects = projects;
        _domainEvents = domainEvents;
        _logger = logger;
    }

    public async Task AcceptAsync(Guid requesterId, Guid applicationId)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        var application = await _applications.GetByIdAsync(applicationId) ??
            throw new InvalidOperationException("The application with the given Id doesn't exist");

        var project = await _projects.GetByIdAsync(application.ProjectId);
        if (project.AuthorId != requester.Id)
        {
            throw new InvalidOperationException("Only the project author can accept applications for their project");
        }

        application.Accept();

        // TODO: wrap in a transaction
        await _applications.UpdateAsync(application);
        await Task.WhenAll(application.DomainEvents.Select(de => _domainEvents.AddAsync(de)));
    }

    public async Task<List<Domain.Applications.Application>> GetReceivedAsync(
        Guid requesterId,
        Guid? projectId = null,
        Guid? positionId = null,
        ApplicationsSortOption applicationsSortOption = ApplicationsSortOption.Default)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        if (projectId is null && positionId is not null)
        {
            throw new InvalidOperationException("Can't retrieve incoming applications by specifying positionId and not the projectId as well");
        }

        if (projectId is not null)
        {
            var project = await _projects.GetByIdAsync((Guid)projectId) ??
                throw new InvalidOperationException("The project with the given Id doesn't exist");

            if (requesterId != project.AuthorId)
            {
                throw new InvalidOperationException("Only the project author can see incoming applications for the specified project");
            }

            if (positionId is not null && project.GetPositionById((Guid)positionId) is null)
            {
                throw new InvalidOperationException("Can't retrieve incoming applications for a project position that doesn't exist");
            }
        }

        // requesterId will be needed if neither projectId nor positionId have been specified
        return await _applications.GetReceivedAsync(
            requesterId,
            projectId,
            positionId,
            applicationsSortOption);
    }

    public async Task<List<Domain.Applications.Application>> GetSentAsync(
        Guid requesterId,
        Guid? projectId = null,
        Guid? positionId = null,
        ApplicationsSortOption applicationsSortOption = ApplicationsSortOption.Default)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        if (projectId is null && positionId is not null)
        {
            throw new InvalidOperationException("Can't retrieve sent applications by specifying positionId and not the projectId as well");
        }

        if (projectId is not null)
        {
            var project = await _projects.GetByIdAsync((Guid)projectId) ??
                throw new InvalidOperationException("The project with the given Id doesn't exist");

            if (positionId is not null && project.GetPositionById((Guid)positionId) is null)
            {
                throw new InvalidOperationException("Can't retrieve sent applications for a project position that doesn't exist");
            }
        }

        return await _applications.GetSentAsync(
            requesterId,
            projectId,
            positionId,
            applicationsSortOption);
    }

    public async Task RejectAsync(Guid requesterId, Guid applicationId)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        var application = await _applications.GetByIdAsync(applicationId) ??
            throw new InvalidOperationException("The application with the given Id doesn't exist");

        var project = await _projects.GetByIdAsync(application.ProjectId);
        if (project.AuthorId != requester.Id)
        {
            throw new InvalidOperationException("Only the project author can reject applications for their project");
        }

        application.Reject();

        await _applications.UpdateAsync(application);
    }

    public async Task RevokeAsync(Guid applicantId, Guid applicationId)
    {
        var application = await _applications.GetByIdAsync(applicationId) ??
            throw new InvalidOperationException("The application with the given Id doesn't exist");

        if (application.ApplicantId != applicantId)
        {
            throw new InvalidOperationException("Only own submitted application can be revoked");
        }

        application.Revoke();

        await _applications.UpdateAsync(application);
    }

    // TODO: consider whether to keep use cases grouped like this
    public async Task RevokeOrRejectAsync(Guid requesterId, Guid applicationId)
    {
        var requester = await _contributors.GetByIdAsync(requesterId) ??
            throw new InvalidOperationException("The contributor with the given Id doesn't exist");

        var application = await _applications.GetByIdAsync(applicationId) ??
            throw new InvalidOperationException("The application with the given Id doesn't exist");

        if (requester.Id == application.ApplicantId)
        {
            await RevokeAsync(requesterId, applicationId);
        }
        else
        {
            await RejectAsync(requesterId, applicationId);
        }
    }

    public async Task<Domain.Applications.Application> SubmitAsync(Guid applicantId, Guid projectId, Guid positionId)
    {
        // TODO: check for existence of all entities involved in all service requests

        var applicant = await _contributors.GetByIdAsync(applicantId) ??
            throw new InvalidOperationException("The user doesn't exist");

        var project = await _projects.GetByIdAsync(projectId) ??
            throw new InvalidOperationException("The project doesn't exist");

        var alreadyApplied = await _applications.GetByApplicantAndPositionAsync(applicantId, positionId) is not null;

        if (alreadyApplied)
        {
            throw new InvalidOperationException("Cant apply for the same position multiple times");
        }

        var application = project.SubmitApplication(applicant.Id, positionId);

        await _applications.AddAsync(application);

        _logger.LogInformation(
            "Applicant {ApplicantId} submitted an application for {PositionId}",
            applicant.Id,
            positionId);

        return application;
    }
}
