using Peer.Domain.Common;

namespace Peer.Domain.Collaborations;
public class Collaboration : AbstractEntity
{
    public Guid CollaboratorId { get; }
    public Guid ProjectId { get; }
    public Guid PositionId { get; }
    public CollaborationStatus Status { get; private set; }

    public bool Terminated =>
        Status is CollaborationStatus.TerminatedByCollaborator
        or CollaborationStatus.TerminatedByProjectAuthor;

    public Collaboration(Guid collaboratorId, Guid projectId, Guid positionId)
    {
        CollaboratorId = collaboratorId;
        ProjectId = projectId;
        PositionId = positionId;
        Status = CollaborationStatus.Active;
    }

    public void TerminateByCollaborator()
    {
        if (Terminated)
        {
            throw new InvalidOperationException("The collaboration has already been terminated");
        }

        Status = CollaborationStatus.TerminatedByCollaborator;
    }

    public void TerminateByAuthor()
    {
        if (Terminated)
        {
            throw new InvalidOperationException("The collaboration has already been terminated");
        }

        Status = CollaborationStatus.TerminatedByProjectAuthor;
    }
}
