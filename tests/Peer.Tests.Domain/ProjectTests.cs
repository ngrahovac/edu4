using System.Diagnostics.CodeAnalysis;
using Peer.Domain.Projects;
using FluentAssertions;
using Peer.Domain.Contributors;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ProjectTests
{
    [Fact]
    public void Cannot_create_a_project_without_any_positions()
    {
        var creatingAProjectWithoutAnyPositions = ()
            => new ProjectsFactory().WithPositions(new List<Position>())
            .Build();

        creatingAProjectWithoutAnyPositions.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Project_position_cannot_be_closed_twice()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student).Build())
            .Build()
        }).Build();

        project.ClosePosition(project.Positions.ElementAt(0).Id);

        var closingAClosedPosition = () => project.ClosePosition(project.Positions.ElementAt(0).Id);

        closingAClosedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Project_position_cannot_be_removed_twice()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student).Build())
            .Build()
        }).Build();

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var removingARemovedPosition = () => project.RemovePosition(project.Positions.ElementAt(0).Id);

        removingARemovedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_close_a_removed_position()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student).Build())
            .Build()
        }).Build();

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var closingARemovedPosition = () => project.ClosePosition(project.Positions.ElementAt(0).Id);

        closingARemovedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_reopen_a_removed_position()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student).Build())
            .Build()
        }).Build();

        project.RemovePosition(project.Positions.ElementAt(0).Id);

        var reopeningARemovedPosition = () => project.ReopenPosition(project.Positions.ElementAt(0).Id);

        reopeningARemovedPosition.Should().Throw<InvalidOperationException>();
    }
}
