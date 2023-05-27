using System.Reflection;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Tests.Application.TestData;
internal class PositionFactory
{
    private string _name = "Test position name";
    private string _description = "Test position description";
    private Hat _requirements = new StudentHat("Software Engineering");
    private bool _open = true;

    public PositionFactory WithName(string name)
    {
        _name = name;
        return this;
    }

    public PositionFactory WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public PositionFactory WithRequirements(Hat requirements)
    {
        _requirements = requirements;
        return this;
    }

    public PositionFactory WithOpen(bool open)
    {
        _open = open;
        return this;
    }

    public Position Build()
    {
        var position = new Position(_name, _description, _requirements);

        SetOpenViaReflection(position);

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
}
