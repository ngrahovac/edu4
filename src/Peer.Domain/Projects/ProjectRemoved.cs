using Peer.Domain.Common;

namespace Peer.Domain.Projects;

public class ProjectRemoved : AbstractDomainEvent
{
    public Guid ProjectId { get; }

    [Obsolete("Used by ORM only")]
    public ProjectRemoved(Guid projectId, DateTime timeRaised)
        : base(timeRaised) => ProjectId = projectId;

    public ProjectRemoved(Project project)
        : base(DateTime.UtcNow) => ProjectId = project.Id;
}
