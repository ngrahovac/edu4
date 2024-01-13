using Peer.Domain.Common;

namespace Peer.Domain.Contributors;

public class ContributorRemoved : AbstractDomainEvent
{
    public Guid ContributorId { get; }

    [Obsolete("Used by ORM only")]
    public ContributorRemoved(Guid contributorId, DateTime timeRaised)
        : base(timeRaised) => ContributorId = contributorId;

    public ContributorRemoved(Contributor contributor)
        : base(DateTime.UtcNow) => ContributorId = contributor.Id;
}
