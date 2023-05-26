using Peer.Domain.Common;

namespace Peer.Domain.Applications;
public class Application : AbstractEntity
{
    public Guid ApplicantId { get; }
    public Guid ProjectId { get; }
    public Guid PositionId { get; }
    public ApplicationStatus Status { get; private set; }

    public Application(Guid applicantId, Guid projectId, Guid positionId)
    {
        ApplicantId = applicantId;
        ProjectId = projectId;
        PositionId = positionId;
        Status = ApplicationStatus.Submitted;
    }

    public void Revoke()
    {
        if (Status is not ApplicationStatus.Submitted)
        {
            throw new InvalidOperationException("Only a submitted application can be revoked");
        }

        Status = ApplicationStatus.Revoked;
    }

    public void Accept()
    {
        if (Status is not ApplicationStatus.Submitted)
        {
            throw new InvalidOperationException("Only a submitted application can be accepted");
        }

        Status = ApplicationStatus.Accepted;
    }
}
