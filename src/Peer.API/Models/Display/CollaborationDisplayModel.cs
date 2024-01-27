using Peer.API.Utils;
using Peer.Domain.Collaborations;

namespace Peer.API.Models.Display;

public class CollaborationDisplayModel
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectUrl { get; set; }
    public Guid PositionId { get; set; }
    public Guid CollaboratorId { get; set; }
    public string CollaboratorUrl { get; set; }
    public string Status { get; set; }

    public CollaborationDisplayModel(Collaboration collaboration)
    {
        Id = collaboration.Id;
        ProjectId = collaboration.ProjectId;
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(collaboration.ProjectId);
        PositionId = collaboration.PositionId;
        CollaboratorId = collaboration.CollaboratorId;
        CollaboratorUrl = ResourceUrlBuilder.BuildContributorUrl(collaboration.CollaboratorId);
        Status = collaboration.Status.ToString();
    }
}
