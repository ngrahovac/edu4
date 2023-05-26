using Microsoft.Extensions.Logging;
using Peer.Application.Contracts;

namespace Peer.Application.Services;
public class ApplicationsService
{
    private readonly IApplicationsRepository _applications;
    private readonly IContributorsRepository _contributors;
    private readonly IProjectsRepository _projects;
    private readonly ILogger<ApplicationsService> _logger;

    public ApplicationsService(
        IApplicationsRepository applications,
        IContributorsRepository contributors,
        IProjectsRepository projects,
        ILogger<ApplicationsService> logger)
    {
        _applications = applications;
        _contributors = contributors;
        _projects = projects;
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
