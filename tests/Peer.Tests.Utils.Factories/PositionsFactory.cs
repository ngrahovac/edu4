using System.Reflection;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Tests.Utils.Factories;

public class PositionsFactory
{
    private string _name = "Test position name";
    private string _description = "Test position description";
    private Hat _requirements = new StudentHat("Software Engineering");
    private bool _removed = false;
    private bool _open = true;

    public PositionsFactory WithName(string name)
    {
        _name = name;
        return this;
    }

    public PositionsFactory WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public PositionsFactory WithRequirements(Hat requirements)
    {
        _requirements = requirements;
        return this;
    }

    public PositionsFactory WithOpen(bool open)
    {
        _open = open;
        return this;
    }

    public PositionsFactory WithRemoved(bool removed)
    {
        _removed = removed;
        return this;
    }

    public Position Build()
    {
        var position = new Position(_name, _description, _requirements);

        SetOpenViaReflection(position);
        SetRemovedViaReflection(position);

        return position;
    }

    private void SetOpenViaReflection(Position position)
    {
        var openProp = typeof(Position).GetProperty(
            nameof(Position.Open),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error setting position being open via reflection");

        openProp.SetValue(position, _open);
    }

    private void SetRemovedViaReflection(Position position)
    {
        var openProp = typeof(Position).GetProperty(
            nameof(Position.Removed),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error setting position being removed via reflection");

        openProp.SetValue(position, _removed);
    }
}
