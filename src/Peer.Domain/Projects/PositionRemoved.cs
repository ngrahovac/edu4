using Peer.Domain.Common;

namespace Peer.Domain.Projects;
public class PositionRemoved : AbstractDomainEvent
{
    public Guid ProjectId { get; }

    public Guid PositionId { get; }

    public PositionRemoved(Project project, Position position)
    {
        ProjectId = project.Id;
        PositionId = position.Id;
    }
}
