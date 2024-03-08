using Peer.Domain.Common;
using Peer.Domain.Contributors;

namespace Peer.Domain.Notifications;
public class AbstractNotification : AbstractAggregateRoot
{
    public Guid ContributorToNotifyId { get; private set; }
    public bool Processed { get; private set; }
    public string Message { get; private set; }


    public AbstractNotification(Contributor contributorToNotify, string message)
    {
        ContributorToNotifyId = contributorToNotify.Id;
        Processed = false;
        Message = message;
    }

    public void Process()
    {
        if (Processed)
        {
            throw new InvalidOperationException("The notification has already been processed");
        }

        Processed = true;
    }
}
