using System.Diagnostics.CodeAnalysis;
using Peer.Domain.Contributors;
using FluentAssertions;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class ContributorTests
{
    [Fact]
    public void A_contributor_can_wear_multiple_distinct_hats()
    {
        var hat1 = HatsFactory.OfType(HatType.Student).Build();
        var hat2 = HatsFactory.OfType(HatType.Academic).Build();

        var creatingAContributorWithMultipleDistinctHats = () => new ContributorsFactory().WithHats(new List<Hat>()
        {
            hat1,
            hat2
        }).Build();


        creatingAContributorWithMultipleDistinctHats.Should().NotThrow();

        var createdContributor = creatingAContributorWithMultipleDistinctHats();
        createdContributor.Hats.Should().BeEquivalentTo(new List<Hat> { hat1, hat2 });
    }

    [Fact]
    public void A_contributor_cannot_wear_duplicate_hats()
    {
        var hat = HatsFactory.OfType(HatType.Student).Build();

        var creatingAContributorWithDuplicateHats = () => new ContributorsFactory().WithHats(new List<Hat>()
        {
            hat,
            hat
        }).Build();


        creatingAContributorWithDuplicateHats.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cannot_update_contact_email_of_a_removed_contributor()
    {
        var hat1 = HatsFactory.OfType(HatType.Student).Build();
        var hat2 = HatsFactory.OfType(HatType.Academic).Build();
        var oldHats = new List<Hat>() { hat1, hat2 };

        var contributor = new ContributorsFactory().WithHats(oldHats)
            .WithRemoved(true)
            .Build();

        var updatingHatsOfARemovedContributor = () => contributor.UpdateHats(new List<Hat>()
        {
            hat1,
            hat2,
            HatsFactory.OfType(HatType.Student)
            .WithAcademicDegree(AcademicDegree.Doctorate)
            .Build()
        });

        updatingHatsOfARemovedContributor.Should().Throw<InvalidOperationException>();
        contributor.Hats.Should().BeEquivalentTo(oldHats);
    }

    [Fact]
    public void Cannot_update_name_of_a_removed_contributor()
    {
        var oldName = "Jane Doe";
        var contributor = new ContributorsFactory().WithFullName(oldName)
            .WithRemoved(true)
            .Build();

        var updatingNameOfARemovedContributor = () => contributor.UpdateFullName("New Name");

        updatingNameOfARemovedContributor.Should().Throw<InvalidOperationException>();
        contributor.FullName.Should().Be(oldName);
    }

    [Fact]
    public void Cannot_update_hats_of_a_removed_contributor()
    {
        var oldName = "Jane Doe";
        var contributor = new ContributorsFactory().WithFullName(oldName)
            .WithRemoved(true)
            .Build();

        var updatingNameOfARemovedContributor = () => contributor.UpdateFullName("New Name");

        updatingNameOfARemovedContributor.Should().Throw<InvalidOperationException>();
        contributor.FullName.Should().Be(oldName);
    }

    [Fact]
    public void Cannot_remove_a_contributor_twice()
    {
        var contributor = new ContributorsFactory().WithRemoved(true)
            .Build();

        var removingAContributorTwice = () => contributor.Remove();

        removingAContributorTwice.Should().Throw<InvalidOperationException>();
    }
}
