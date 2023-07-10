using Peer.Application.Contracts;
using Peer.Domain.Collaborations;
using Peer.Domain.Contributors;
using Peer.Domain.Notifications;

namespace Peer.Application.Handlers;
public class ApplicationAcceptedHandler
{
    private readonly IApplicationsRepository _applications;
    private readonly ICollaborationsRepository _collaborations;
    private readonly IContributorsRepository _contributors;
    private readonly INotificationsRepository _notifications;

    public ApplicationAcceptedHandler(
        IApplicationsRepository applications,
        ICollaborationsRepository collaborations,
        IContributorsRepository contributors,
        INotificationsRepository notifications)
    {
        _applications = applications;
        _collaborations = collaborations;
        _contributors = contributors;
        _notifications = notifications;
    }

    public async Task MakeApplicantACollaboratorOnTheProjectAsync(Guid applicationId)
    {
        var acceptedApplication = await _applications.GetByIdAsync(applicationId);

        var collaboration = new Collaboration(
            acceptedApplication.ApplicantId,
            acceptedApplication.ProjectId,
            acceptedApplication.PositionId);

        await _collaborations.AddAsync(collaboration);
    }

    public async Task NotifyApplicantAboutTheirApplicationBeingAcceptedAsync(Guid applicationId)
    {
        var acceptedApplication = await _applications.GetByIdAsync(applicationId);
        var applicant = await _contributors.GetByIdAsync(acceptedApplication.ApplicantId);

        var notification = new OwnApplicationAccepted(applicant, acceptedApplication);

        await _notifications.AddAsync(notification);
    }
}
