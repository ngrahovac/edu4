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
    public bool Removed { get; private set; }

    private readonly List<Position> _positions;
    public IReadOnlyCollection<Position> Positions
        => _positions.ToList();

    public Project(
        string title,
        string description,
        Guid authorId,
        DateTime datePosted,
        ICollection<Position> positions)
    {
        if (positions.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish a project without any positions");
        }

        _positions = positions.ToList();

        DatePosted = datePosted;
        Title = title;
        Description = description;
        AuthorId = authorId;
        Removed = false;
    }

    public bool IsRecommendedFor(Contributor user) =>
        !Removed &&
        user.Id != AuthorId &&
        Positions.Any(p => user.Hats.Any(h => h.Fits(p.Requirements)));

    public bool WasPublishedBy(Contributor user) =>
        user.Id == AuthorId;

    public void AddPosition(string name, string description, Hat requirements)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Can't add a position on a removed project");
        }

        if (Positions.Any(p => p.Name.Equals(name, StringComparison.Ordinal) && p.Requirements.Equals(requirements)))
        {
            throw new InvalidOperationException("A project cannot have two positions with the same name and requirements");
        }

        var position = new Position(name, description, requirements);
        _positions.Add(position);
    }

    public void UpdateDetails(string title, string description)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Can't update the details of a removed project");
        }

        Title = title;
        Description = description;
    }

    public Application SubmitApplication(Guid applicantId, Guid positionId)
    {
        var position = GetNonRemovedPositionById(positionId) ??
            throw new InvalidOperationException("Can't apply for a position that doesn't exist");

        if (Removed)
        {
            throw new InvalidOperationException("Can't apply for a position on a removed project");
        }

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
        if (Removed)
        {
            throw new InvalidOperationException("Can't close a position on a removed project");
        }

        var position = GetNonRemovedPositionById(positionId) ??
            throw new InvalidOperationException("Can't close a position that doesn't exist");

        position.Close();
        RaiseDomainEvent(new PositionClosed(this, position));
    }

    public void ReopenPosition(Guid positionId)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Can't reopen a position on a removed project");
        }

        var position = GetNonRemovedPositionById(positionId) ??
            throw new InvalidOperationException("Can't reopen a position that doesn't exist");

        position.Reopen();
    }

    public void RemovePosition(Guid positionId)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Can't remove a position on a removed project");
        }

        var position = GetNonRemovedPositionById(positionId) ??
            throw new InvalidOperationException("Can't remove a position that doesn't exist");

        position.Remove();
        RaiseDomainEvent(new PositionRemoved(this, position));
    }

    public Position? GetPositionById(Guid positionId)
        => GetNonRemovedPositionById(positionId);

    public void Remove()
    {
        if (Removed)
        {
            throw new InvalidOperationException("The project has already been removed");
        }

        var nonRemovedPositions = Positions.Where(p => !p.Removed);
        nonRemovedPositions.ToList().ForEach(p => p.Remove());

        Removed = true;
        RaiseDomainEvent(new ProjectRemoved(this));
    }

    private Position? GetNonRemovedPositionById(Guid positionId) =>
        _positions.FirstOrDefault(p => p.Id == positionId && !p.Removed);
}
