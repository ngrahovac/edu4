using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ApplicationTests
{
    [Fact]
    public void A_submitted_application_can_be_revoked()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var revokeSubmittedApplication = () => application.Revoke();

        revokeSubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Revoked);
    }

    [Fact]
    public void Author_cannot_submit_an_application_for_a_position_on_own_project()
    {
        var authorId = Guid.NewGuid();
        var project = new Project(
            string.Empty,
            string.Empty,
            authorId,
            new List<Position>
            {
                new Position(
                    string.Empty,
                    string.Empty,
                    new StudentHat("Software Engineering"))
            });

        var submitApplicationForAPositionOnOwnProject = () => project.SubmitApplication(
            authorId,
            project.Positions.ElementAt(0).Id);

        submitApplicationForAPositionOnOwnProject.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cant_apply_for_a_closed_position()
    {
        var project = new Project(
            string.Empty,
            string.Empty,
            Guid.NewGuid(),
            new List<Position>
            {
                new Position(
                    string.Empty,
                    string.Empty,
                    new StudentHat("Software Engineering"))
            });

        project.ClosePosition(project.Positions.ElementAt(0).Id);

        var applyForAClosedPosition = () => project.SubmitApplication(
            Guid.NewGuid(),
            project.Positions.ElementAt(0).Id);

        applyForAClosedPosition.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_submitted_application_can_be_accepted()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var acceptSubmittedApplication = () => application.Accept();

        acceptSubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Accepted);
    }

    [Fact]
    public void A_revoked_application_cannot_be_accepted()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        application.Revoke();

        var acceptRevokedApplication = () => application.Accept();

        acceptRevokedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_submitted_application_can_be_rejected()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var rejectSubmittedApplication = () => application.Reject();

        rejectSubmittedApplication.Should().NotThrow();
        application.Status.Should().Be(Peer.Domain.Applications.ApplicationStatus.Rejected);
    }

    [Fact]
    public void A_revoked_application_cannot_be_rejected()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        application.Revoke();

        var rejectRevokedApplication = () => application.Reject();

        rejectRevokedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void An_accepted_application_cannot_be_rejected()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        application.Accept();

        var rejectAcceptedApplication = () => application.Reject();

        rejectAcceptedApplication.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void A_rejected_application_cannot_be_rejected_for_the_second_time()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        application.Reject();

        var rejectRejectedApplication = () => application.Accept();

        rejectRejectedApplication.Should().Throw<InvalidOperationException>();
    }
}
