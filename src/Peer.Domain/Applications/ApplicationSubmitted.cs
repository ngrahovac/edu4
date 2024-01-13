using Peer.Domain.Common;

namespace Peer.Domain.Applications;

public class ApplicationSubmitted : AbstractDomainEvent
{
    public Guid ApplicationId { get; }

    [Obsolete("Used by ORM only")]
    public ApplicationSubmitted(Guid applicationId, DateTime timeRaised)
        : base(timeRaised) => ApplicationId = applicationId;

    public ApplicationSubmitted(Application application)
        : base(DateTime.UtcNow) => ApplicationId = application.Id;
}
