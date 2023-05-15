using edu4.Domain.Common;
using edu4.Domain.Contributors;

namespace edu4.Domain.Projects;
public class Project : AbstractAggregateRoot
{
    public DateTime DatePosted { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Author Author { get; }

    private readonly List<Position> _positions;
    public IReadOnlyCollection<Position> Positions
        => _positions.ToList();


    public Project(
        string title,
        string description,
        Author author,
        ICollection<Position> positions)
    {
        DatePosted = DateTime.UtcNow;
        Title = title;
        Description = description;
        Author = author;

        if (positions.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish a project without any positions");
        }

        _positions = positions.ToList();
    }

    public bool IsRecommendedFor(User user) =>
        user.Id != Author.Id &&
        Positions.Any(p => user.Hats.Any(h => h.Fits(p.Requirements)));

    public bool IsAuthoredBy(User user) =>
        user.Id == Author.Id;

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
}
