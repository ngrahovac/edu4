using Peer.Domain.Common;

namespace Peer.Domain.Projects;
public class PositionClosed : AbstractDomainEvent
{
    public Guid ProjectId { get; }
    public Guid PositionId { get; }

    public PositionClosed(Project project, Position position)
    {
        ProjectId = project.Id;
        PositionId = position.Id;
    }
}
