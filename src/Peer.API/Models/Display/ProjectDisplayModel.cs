using Peer.API.Utils;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class ProjectDisplayModel
{
    public string ProjectUrl { get; set; }
    public DateTime DatePosted { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AuthorUrl { get; set; }
    public bool Authored { get; set; }
    public bool Recommended { get; set; }
    public IReadOnlyCollection<PositionDisplayModel> Positions { get; set; }

    public ProjectDisplayModel(Project project, Contributor requester)
    {
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(project.Id);
        Title = project.Title;
        Description = project.Description;
        AuthorUrl = ResourceUrlBuilder.BuildContributorUrl(requester.Id);
        Authored = project.WasPublishedBy(requester);
        Recommended = project.IsRecommendedFor(requester);
        Positions = project.Positions
            .Select(pp => new PositionDisplayModel(project, pp, requester))
            .ToList();
    }
}
