using System.Diagnostics.CodeAnalysis;
using Peer.Domain.Projects;
using FluentAssertions;

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
}
