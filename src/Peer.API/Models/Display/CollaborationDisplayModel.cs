using Peer.API.Utils;
using Peer.Domain.Collaborations;

namespace Peer.API.Models.Display;

public class CollaborationDisplayModel
{
    public string ProjectUrl { get; set; }
    public Guid PositionId { get; set; }
    public string Status { get; set; }

    public CollaborationDisplayModel(Collaboration collaboration)
    {
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(collaboration.ProjectId);
        PositionId = collaboration.PositionId;
        Status = collaboration.Status.ToString();
    }
}
