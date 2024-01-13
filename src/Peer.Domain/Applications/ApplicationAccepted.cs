using Peer.Domain.Common;

namespace Peer.Domain.Applications;

public class ApplicationAccepted : AbstractDomainEvent
{
    public Guid ApplicationId { get; }

    [Obsolete("Used by ORM only")]
    public ApplicationAccepted(Guid applicationId, DateTime timeRaised)
        : base(timeRaised) => ApplicationId = applicationId;

    public ApplicationAccepted(Application application)
        : base(DateTime.UtcNow) => ApplicationId = application.Id;
}
