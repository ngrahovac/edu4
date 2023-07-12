using Peer.Domain.Projects;

namespace Peer.Tests.Utils.Factories;

public class ProjectsFactory
{
    private string _title = "Test project title";
    private string _description = "Test project description";
    private Guid _authorId = Guid.NewGuid();
    private List<Position> _positions = new();


    public ProjectsFactory WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ProjectsFactory WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProjectsFactory WithAuthorId(Guid authorId)
    {
        _authorId = authorId;
        return this;
    }

    public ProjectsFactory WithPositions(List<Position> positions)
    {
        _positions = positions;
        return this;
    }

    public Project Build()
    {
        return new Project(
            _title,
            _description,
            _authorId,
            _positions);
    }
}
