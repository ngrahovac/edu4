using Peer.Domain.Common;

namespace Peer.Domain.Applications;
public class ApplicationAccepted : AbstractDomainEvent
{
    public Guid ApplicationId { get; }

    public ApplicationAccepted(Application application) => ApplicationId = application.Id;
}
