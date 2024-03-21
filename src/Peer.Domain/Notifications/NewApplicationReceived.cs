using Peer.Domain.Contributors;

namespace Peer.Domain.Notifications;
public class NewApplicationReceived : AbstractNotification
{
    public Guid ApplicationId { get; }

    public NewApplicationReceived(Contributor contributorToNotify, Applications.Application application, DateTime timestamp)
        : base(contributorToNotify, "New incoming application", timestamp)
        => ApplicationId = application.Id;
}
