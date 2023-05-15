using edu4.Domain.Contributors;
using edu4.Domain.Projects;

namespace edu4.Application.Tests.TestData;
internal class PositionFactory
{
    private string _name = "Test position name";
    private string _description = "Test position description";
    private Hat _requirements = new StudentHat("Software Engineering");

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

    public Position Build() => new(_name, _description, _requirements);
}
