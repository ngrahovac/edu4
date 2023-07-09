using Peer.Domain.Common;

namespace Peer.Domain.Applications;

public class ApplicationSubmitted : AbstractDomainEvent
{
    public Guid ApplicationId { get; }

    public ApplicationSubmitted(Application application) => ApplicationId = application.Id;
}
