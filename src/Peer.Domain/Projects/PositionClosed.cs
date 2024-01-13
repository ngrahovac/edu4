using Peer.Domain.Common;

namespace Peer.Domain.Projects;

public class PositionClosed : AbstractDomainEvent
{
    public Guid ProjectId { get; }
    public Guid PositionId { get; }

    [Obsolete("Used by ORM only")]
    public PositionClosed(Guid projectId, Guid positionId, DateTime timeRaised)
        : base(timeRaised)
    {
        ProjectId = projectId;
        PositionId = positionId;
    }

    public PositionClosed(Project project, Position position)
        : base(DateTime.UtcNow)
    {
        ProjectId = project.Id;
        PositionId = position.Id;
    }
}
