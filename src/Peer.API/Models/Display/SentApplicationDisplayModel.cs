using Peer.API.Utils;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class SentApplicationDisplayModel
{
    public Guid Id { get; set; }
    public Guid ApplicantId { get; set; }
    public string ApplicantUrl { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectUrl { get; set; }
    public Guid PositionId { get; set; }
    public string DateSubmitted { get; set; }
    public string Status { get; set; }
    public bool Own { get; set; }
    public ProjectDisplayModel Project { get; set; }

    public SentApplicationDisplayModel(
        Domain.Applications.Application application,
        Contributor requester,
        Project project)
    {
        Id = application.Id;
        ApplicantId = application.ApplicantId;
        ApplicantUrl = ResourceUrlBuilder.BuildContributorUrl(application.ApplicantId);
        ProjectId = application.ProjectId;
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(application.ProjectId);
        PositionId = application.PositionId;
        DateSubmitted = application.DateSubmitted.ToShortDateString();
        Status = application.Status.ToString();
        Own = requester.Id.Equals(ApplicantId);
        Project = new ProjectDisplayModel(project, requester);
    }
}
