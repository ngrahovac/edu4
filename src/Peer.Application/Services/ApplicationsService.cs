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
