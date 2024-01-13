using Peer.API.Utils;
using Peer.Domain.Collaborations;

namespace Peer.API.Models.Display;

public class CollaborationDisplayModel
{
    public Guid Id { get; set; }
    public string ProjectUrl { get; set; }
    public Guid PositionId { get; set; }
    public string Status { get; set; }

    public CollaborationDisplayModel(Collaboration collaboration)
    {
        Id = collaboration.Id;
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(collaboration.ProjectId);
        PositionId = collaboration.PositionId;
        Status = collaboration.Status.ToString();
    }
}
