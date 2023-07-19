using Peer.API.Utils;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class PositionDisplayModel
{
    public string PositionUrl { get; set; }
    public DateTime DatePosted { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public HatDisplayModel Requirements { get; set; }
    public bool Recommended { get; set; }

    public PositionDisplayModel(
        Project project,
        Position position,
        Contributor requester)
    {
        PositionUrl = ResourceUrlBuilder.BuildPositionUrl(project.Id, position.Id);
        DatePosted = position.DatePosted;
        Name = position.Name;
        Description = position.Description;
        Requirements = new HatDisplayModel(position.Requirements);
        Recommended = position.IsRecommendedFor(requester);
    }
}
