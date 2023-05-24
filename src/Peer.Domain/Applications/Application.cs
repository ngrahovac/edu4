using Peer.Domain.Common;

namespace Peer.Domain.Applications;
public class Application : AbstractEntity
{
    public Guid ApplicantId { get; }
    public Guid ProjectId { get; }
    public Guid PositionId { get; }

    public Application(Guid applicantId, Guid projectId, Guid positionId)
    {
        ApplicantId = applicantId;
        ProjectId = projectId;
        PositionId = positionId;
    }
}
