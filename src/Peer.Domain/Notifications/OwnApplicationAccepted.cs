using Peer.Domain.Contributors;

namespace Peer.Domain.Notifications;

public class OwnApplicationAccepted : AbstractNotification
{
    public Guid ApplicationId { get; }
    public OwnApplicationAccepted(Contributor contributorToNotify, Applications.Application application)
        : base(contributorToNotify, "Your application has been accepted")
        => ApplicationId = application.Id;
}
