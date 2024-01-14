using Peer.API.Utils;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class ProjectDisplayModel
{
    public Guid Id { get; set; }
    public string ProjectUrl { get; set; }
    public string DatePosted { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AuthorUrl { get; set; }
    public bool Authored { get; set; }
    public bool Recommended { get; set; }
    public IReadOnlyCollection<PositionDisplayModel> Positions { get; set; }
    public string CollaborationsUrl { get; set; }
    public DurationDisplayModel Duration { get; set; }

    public ProjectDisplayModel(Project project, Contributor requester)
    {
        Id = project.Id;
        ProjectUrl = ResourceUrlBuilder.BuildProjectUrl(project.Id);
        Title = project.Title;
        Description = project.Description;
        AuthorUrl = ResourceUrlBuilder.BuildContributorUrl(project.AuthorId);
        Authored = project.WasPublishedBy(requester);
        DatePosted = project.DatePosted.ToShortDateString();
        Recommended = project.IsRecommendedFor(requester);
        Positions = project.Positions
            .Where(pp => !pp.Removed)
            .Select(pp => new PositionDisplayModel(project, pp, requester))
            .ToList();
        CollaborationsUrl = ResourceUrlBuilder.BuildProjectCollaborationsUrl(project.Id);
        Duration = new(project.Duration);
    }
}
