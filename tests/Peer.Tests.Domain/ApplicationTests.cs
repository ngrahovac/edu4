using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ApplicationTests
{
    [Fact]
    public void A_submitted_application_can_become_revoked()
    {
        var application = new Peer.Domain.Applications.Application(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var revokeApplication = () => application.Revoke();

        revokeApplication.Should().NotThrow();
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

    // TODO: test all domain invariants; use test data factories?
}