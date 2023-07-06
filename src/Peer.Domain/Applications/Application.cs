using Peer.Domain.Common;

namespace Peer.Domain.Applications;
public class Application : AbstractAggregateRoot
{
    public Guid ApplicantId { get; }
    public Guid ProjectId { get; }
    public Guid PositionId { get; }
    public ApplicationStatus Status { get; private set; }
    public DateTime DateSubmitted { get; private set; }

    public Application(Guid applicantId, Guid projectId, Guid positionId)
    {
        ApplicantId = applicantId;
        ProjectId = projectId;
        PositionId = positionId;
        Status = ApplicationStatus.Submitted;
        DateSubmitted = DateTime.Now;
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
        RaiseDomainEvent(new ApplicationAccepted(this));
    }

    public void Reject()
    {
        if (Status is not ApplicationStatus.Submitted)
        {
            throw new InvalidOperationException("Only a submitted application can be rejected");
        }

        Status = ApplicationStatus.Rejected;
    }

    public void Remove()
    {
        if (Status is not ApplicationStatus.Removed)
        {
            throw new InvalidOperationException("The application has already been removed");
        }

        Status = ApplicationStatus.Removed;
    }
}
