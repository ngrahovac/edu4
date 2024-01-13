using Peer.Domain.Common;

namespace Peer.Domain.Contributors;
public class ContributorRemoved : AbstractDomainEvent
{
    public Guid ContributorId { get; }

    public ContributorRemoved(Contributor contributor) => ContributorId = contributor.Id;
}
