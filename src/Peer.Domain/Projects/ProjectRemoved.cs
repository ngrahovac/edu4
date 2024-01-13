using Peer.Domain.Common;

namespace Peer.Domain.Projects;
public class ProjectRemoved : AbstractDomainEvent
{
    public Guid ProjectId { get; }

    public ProjectRemoved(Project project) => ProjectId = project.Id;
}
