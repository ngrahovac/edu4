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
    public void A_removed_project_cannot_be_recommended()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var contributor = new ContributorsFactory().WithHats(new List<Hat>()
        {
            HatsFactory.OfType(HatType.Student)
            .WithStudyField("Computer Science")
            .WithAcademicDegree(AcademicDegree.Bachelors)
            .Build()
        }).Build();

        var recommended = project.IsRecommendedFor(contributor);

        recommended.Should().BeFalse();
    }

    [Fact]
    public void Cannot_add_a_position_to_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var newPositionRequirements = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Masters)
                .Build();

        var addingAPositionToARemovedProject = () => project.AddPosition(
            "test name",
            "test description",
            newPositionRequirements);

        addingAPositionToARemovedProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_update_the_details_of_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var updatingDetailsOfARemovedProject = () => project.UpdateDetails("new title", "new description");

        updatingDetailsOfARemovedProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_submit_an_application_for_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var applicantId = Guid.NewGuid();

        var submittingAnApplicationForARemovedProject = () => project.SubmitApplication(
            applicantId, project.Positions.ElementAt(0).Id);

        submittingAnApplicationForARemovedProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_close_a_position_on_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var closingAPositionOnARemovedProject = () => project.ClosePosition(
            project.Positions
            .ElementAt(0)
            .Id);

        closingAPositionOnARemovedProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_reopen_a_position_on_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).Build();

        project.Remove();

        var reopeningAPositionOnARemovedProject = () => project.ReopenPosition(
            project.Positions
            .ElementAt(0)
            .Id);

        reopeningAPositionOnARemovedProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_remove_a_position_on_a_removed_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var removingAPositionOnARemovedProject = () => project.RemovePosition(
            project.Positions
            .ElementAt(0)
            .Id);

        removingAPositionOnARemovedProject.Should().Throw<InvalidOperationException>();
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


    [Fact]
    public void Cannot_remove_a_project_twice()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRequirements(
                HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build())
            .Build()
        }).WithRemoved(true)
        .Build();

        var removingARemovedProject = () => project.Remove();

        removingARemovedProject.Should().Throw<InvalidOperationException>();
    }
}
