using Peer.Domain.Applications;

namespace Peer.API.Models.Display;

public class ApplicationDisplayModel
{
    public Guid ApplicationId { get; set; }
    public Guid ApplicantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PositionId { get; set; }
    public DateTime DateSubmitted { get; set; }
    public ApplicationStatus Status { get; set; }

    public ApplicationDisplayModel(Domain.Applications.Application application)
    {
        ApplicationId = application.Id;
        ApplicantId = application.ApplicantId;
        ProjectId = application.ProjectId;
        PositionId = application.PositionId;
        DateSubmitted = application.DateSubmitted;
        Status = application.Status;
    }
}
