using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Peer.Domain.Projects;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ApplicationTests
{
    [Fact]
    public void A_submitted_application_can_be_revoked()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Submitted)
            .Build();

        var revokingASubmittedApplication = () => application.Revoke();

        revokingASubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Revoked);
    }

    [Fact]
    public void Author_cannot_apply_for_a_position_on_own_project()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().Build()
        }).WithAuthorId(Guid.NewGuid())
        .Build();

        var applyingForOwnProject = () => project.SubmitApplication(
            project.AuthorId,
            project.Positions.ElementAt(0).Id);

        applyingForOwnProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cant_apply_for_a_closed_position()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithOpen(false)
            .Build()
        }).Build();

        var applyingForAClosedPosition = () => project.SubmitApplication(
            Guid.NewGuid(),
            project.Positions.ElementAt(0).Id);

        applyingForAClosedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cant_apply_for_a_removed_position()
    {
        var project = new ProjectsFactory().WithPositions(new List<Position>()
        {
            new PositionsFactory().WithRemoved(true).Build()
        }).Build();

        var applyingForARemovedPosition = () => project.SubmitApplication(
            Guid.NewGuid(),
            project.Positions.ElementAt(0).Id);

        applyingForARemovedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_submitted_application_can_be_accepted()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Submitted)
            .Build();

        var acceptingASubmittedApplication = () => application.Accept();

        acceptingASubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Accepted);
    }

    [Fact]
    public void A_revoked_application_cannot_be_accepted()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Revoked)
            .Build();

        var acceptingARevokedApplication = () => application.Accept();

        acceptingARevokedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_rejected_application_cannot_be_accepted()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Rejected)
            .Build();

        var acceptingARejectedApplication = () => application.Accept();

        acceptingARejectedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_removed_application_cannot_be_accepted()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Removed)
            .Build();

        var acceptingARemovedApplication = () => application.Accept();

        acceptingARemovedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void An_application_cannot_be_accepted_twice()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Accepted)
            .Build();

        var acceptingAnApplicationTwice = () => application.Accept();

        acceptingAnApplicationTwice.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_submitted_application_can_be_rejected()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Submitted)
            .Build();

        var rejectingASubmittedApplication = () => application.Reject();

        rejectingASubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Rejected);
    }

    [Fact]
    public void A_revoked_application_cannot_be_rejected()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Revoked)
            .Build();

        var rejectingARevokedApplication = () => application.Reject();

        rejectingARevokedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void An_accepted_application_cannot_be_rejected()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Accepted)
            .Build();

        var rejectingAnAcceptedApplication = () => application.Reject();

        rejectingAnAcceptedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void An_application_cannot_be_rejected_twice()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Rejected)
            .Build();

        var rejectingAnApplicationTwice = () => application.Reject();

        rejectingAnApplicationTwice.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_removed_application_cannot_be_rejected()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Removed)
            .Build();

        var acceptingARemovedApplication = () => application.Accept();

        acceptingARemovedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void An_application_cannot_be_removed_twice()
    {
        var application = new ApplicationsFactory().WithStatus(Peer.Domain.Applications.ApplicationStatus.Removed)
            .Build();

        var removingAnApplicationTwice = () => application.Remove();

        removingAnApplicationTwice.Should().Throw<InvalidOperationException>();
    }
}
