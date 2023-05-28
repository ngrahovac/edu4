using Peer.Domain.Applications;
using Peer.Domain.Common;
using Peer.Domain.Contributors;

namespace Peer.Domain.Projects;
public class Project : AbstractAggregateRoot
{
    public DateTime DatePosted { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid AuthorId { get; }

    private readonly List<Position> _positions;
    public IReadOnlyCollection<Position> Positions
        => _positions.ToList();

    public Project(
        string title,
        string description,
        Guid authorId,
        ICollection<Position> positions)
    {
        DatePosted = DateTime.UtcNow;
        Title = title;
        Description = description;
        AuthorId = authorId;

        if (positions.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish a project without any positions");
        }

        _positions = positions.ToList();
    }

    public bool IsRecommendedFor(Contributor user) =>
        user.Id != AuthorId &&
        Positions.Any(p => user.Hats.Any(h => h.Fits(p.Requirements)));

    public bool WasPublishedBy(Contributor user) =>
        user.Id == AuthorId;

    public void AddPosition(string name, string description, Hat requirements)
    {
        if (Positions.Any(p => p.Name.Equals(name, StringComparison.Ordinal) && p.Requirements.Equals(requirements)))
        {
            throw new InvalidOperationException("A project cannot have two positions with the same name and requirements");
        }

        var position = new Position(name, description, requirements);
        _positions.Add(position);
    }

    public void UpdateDetails(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public Application SubmitApplication(Guid applicantId, Guid positionId)
    {
        var position = GetById(positionId) ??
            throw new InvalidOperationException("Can't apply for a position that doesn't exist");

        if (!position.Open)
        {
            throw new InvalidOperationException("Can't apply for a closed position");
        }

        if (position.Removed)
        {
            throw new InvalidOperationException("Can't apply for a removed position");
        }

        if (applicantId == AuthorId)
        {
            throw new InvalidOperationException("The author can't apply for a position on own project");
        }

        return new Application(applicantId, Id, positionId);
    }

    public void ClosePosition(Guid positionId)
    {
        var position = GetById(positionId) ??
            throw new InvalidOperationException("Can't close a position that doesn't exist");

        position.Close();
    }

    public void ReopenPosition(Guid positionId)
    {
        var position = GetById(positionId) ??
            throw new InvalidOperationException("Can't reopen a position that doesn't exist");

        position.Reopen();
    }

    public void RemovePosition(Guid positionId)
    {
        var position = GetById(positionId) ??
            throw new InvalidOperationException("Can't remove a position that doesn't exist");

        position.Remove();
    }

    public Position? GetPositionById(Guid positionId)
        => GetById(positionId);

    private Position? GetById(Guid positionId) =>
        _positions.FirstOrDefault(p => p.Id == positionId);
}
