using Peer.Application.Contracts;
using Peer.Domain.Notifications;

namespace Peer.Application.Handlers;

public class ApplicationSubmittedHandler
{
    private readonly IApplicationsRepository _applications;
    private readonly INotificationsRepository _notifications;
    private readonly IProjectsRepository _projects;
    private readonly IContributorsRepository _contributors;

    public ApplicationSubmittedHandler(
        IApplicationsRepository applications,
        INotificationsRepository notifications,
        IProjectsRepository projects,
        IContributorsRepository contributors)
    {
        _applications = applications;
        _notifications = notifications;
        _projects = projects;
        _contributors = contributors;
    }
    public async Task NotifyProjectAuthorAboutNewIncomingApplicationAsync(Guid applicationId)
    {
        var submittedApplication = await _applications.GetByIdAsync(applicationId);
        var project = await _projects.GetByIdAsync(submittedApplication.ProjectId);
        var author = await _contributors.GetByIdAsync(project.AuthorId);

        var notification = new NewApplicationReceived(author, submittedApplication);

        await _notifications.AddAsync(notification);
    }
}
