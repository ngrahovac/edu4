using System.Reflection;
using Peer.Domain.Projects;

namespace Peer.Tests.Utils.Factories;

public class ProjectsFactory
{
    private string _title = "Test project title";
    private string _description = "Test project description";
    private Guid _authorId = Guid.NewGuid();
    private List<Position> _positions = new();
    private bool _removed;
    private DateTime _datePosted = DateTime.UtcNow.Date;

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

    public ProjectsFactory WithRemoved(bool removed)
    {
        _removed = removed;
        return this;
    }

    public ProjectsFactory WithDatePosted(DateTime datePosted)
    {
        _datePosted = datePosted;
        return this;
    }

    public Project Build()
    {
        var project = new Project(
            _title,
            _description,
            _authorId,
            _datePosted,
            _positions);

        if (_removed)
        {
            MakeRemovedViaReflection(project);
        }

        return project;
    }

    private void MakeRemovedViaReflection(Project project)
    {
        var removedProp = typeof(Project).GetProperty(
            nameof(Project.Removed),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error making the project removed via reflection");

        removedProp.SetValue(project, true);
    }
}
