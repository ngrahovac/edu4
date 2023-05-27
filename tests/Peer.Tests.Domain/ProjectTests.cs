using System.Diagnostics.CodeAnalysis;
using Peer.Domain.Projects;
using FluentAssertions;
using Peer.Domain.Contributors;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ProjectTests
{
    [Fact]
    public void Cannot_create_a_project_without_any_positions()
    {
        var action = ()
            => _ = new Project(
                string.Empty,
                string.Empty,
                Guid.NewGuid(),
                new List<Position>());

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Project_position_cannot_be_closed_twice()
    {
        var project = new Project(
                string.Empty,
                string.Empty,
                Guid.NewGuid(),
                new List<Position>()
                {
                    new Position("test", "test", new AcademicHat("Computer Science"))
                });

        project.ClosePosition(project.Positions.ElementAt(0).Id);

        var closeThePositionAgain = () => project.ClosePosition(project.Positions.ElementAt(0).Id);

        closeThePositionAgain.Should().Throw<InvalidOperationException>();
    }
}
