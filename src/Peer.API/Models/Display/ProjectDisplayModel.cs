using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.API.Models.Display;

public class ProjectDisplayModel
{
    public Guid Id { get; set; }
    public DateTime DatePosted { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid AuthorId { get; set; }
    public bool Authored { get; set; }
    public bool Recommended { get; set; }
    public IReadOnlyCollection<PositionDisplayModel> Positions { get; set; }

    public ProjectDisplayModel(Project project, Contributor requester)
    {
        Id = project.Id;
        DatePosted = project.DatePosted;
        Title = project.Title;
        Description = project.Description;
        AuthorId = project.AuthorId;
        Authored = project.WasPublishedBy(requester);
        Recommended = project.IsRecommendedFor(requester);
        Positions = project.Positions.Select(p => new PositionDisplayModel(p, requester)).ToList();
    }
}
