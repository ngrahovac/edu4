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

    [Fact]
    public void Project_position_cannot_be_removed_twice()
    {
        var project = new Project(
                string.Empty,
                string.Empty,
                Guid.NewGuid(),
                new List<Position>()
                {
                    new Position("test", "test", new AcademicHat("Computer Science"))
                });

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var removeThePositionAgain = () => project.RemovePosition(project.Positions.ElementAt(0).Id);

        removeThePositionAgain.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_close_a_removed_position()
    {
        var project = new Project(
                string.Empty,
                string.Empty,
                Guid.NewGuid(),
                new List<Position>()
                {
                    new Position("test", "test", new AcademicHat("Computer Science"))
                });

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var closeARemovedPosition = () => project.ClosePosition(project.Positions.ElementAt(0).Id);

        closeARemovedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_reopen_a_removed_position()
    {
        var project = new Project(
                string.Empty,
                string.Empty,
                Guid.NewGuid(),
                new List<Position>()
                {
                    new Position("test", "test", new AcademicHat("Computer Science"))
                });

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var reopenARemovedPosition = () => project.ReopenPosition(project.Positions.ElementAt(0).Id);

        reopenARemovedPosition.Should().Throw<InvalidOperationException>();
    }
}
