using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Peer.Application.Contracts;
using Peer.Application.Models;
using Peer.Application.Services;
using Peer.Domain.Contributors;
using Peer.Infrastructure;
using Peer.Tests.Application.TestData;

namespace Peer.Tests.Application;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class ContributorsServiceTests
{
    [Fact]
    public async void Successfully_signs_up_a_new_user_providing_valid_email_account_id_and_hat_list()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var users = new MongoDbContributorsRepository(config);
        var accountManagementMock = new Mock<IAccountManagementService>(MockBehavior.Strict);
        accountManagementMock.Setup(s => s.MarkUserSignedUpAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var sut = new ContributorsService(
            users,
            accountManagementMock.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var accountId = "google-oauth2|0";
        var fullName = "John Doe";
        var contactEmail = "mail@example.com";
        var hatData = new List<HatDTO>
        {
            new(HatType.Student, new Dictionary<string, object>()
                {
                    { nameof(StudentHat.StudyField), "Computer Science" },
                    { nameof(StudentHat.AcademicDegree),  3 }
                }),
            new(HatType.Academic, new Dictionary<string, object>()
                {
                    {"ResearchField", "Distributed Systems" }
                })
        };

        // ACT
        var signedUpUser = await sut.SignUpAsync(
            accountId,
            fullName,
            contactEmail,
            hatData);

        // ASSERT
        // assert user got assigned id by db driver
        signedUpUser.Id.Should().NotBe(Guid.Empty);

        // assert user was marked as signed up
        accountManagementMock.Verify(s => s.MarkUserSignedUpAsync(accountId), Times.Once);

        // assert db state through repository
        var retrievedUser = await users.GetByIdAsync(signedUpUser.Id);

        retrievedUser!.Id.Should().Be(signedUpUser.Id);
        retrievedUser.AccountId.Should().Be(accountId);
        retrievedUser.FullName.Should().Be(fullName);
        retrievedUser.ContactEmail.Should().Be(contactEmail);
        retrievedUser.Hats.Should().BeEquivalentTo(new List<Hat>()
        {
            new AcademicHat("Distributed Systems"),
            new StudentHat("Computer Science", AcademicDegree.Doctorate)
        });
    }

    [Fact]
    public async void Throws_if_signing_up_a_user_with_account_id_belonging_to_an_existing_user()
    {
        // arrange
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var users = new MongoDbContributorsRepository(config);
        var accountManagementMock = new Mock<IAccountManagementService>(MockBehavior.Strict);
        accountManagementMock.Setup(s => s.MarkUserSignedUpAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var sut = new ContributorsService(
            users,
            accountManagementMock.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var accountId = "google-oauth2|0";
        var fullName = "John Doe";
        var contactEmail = "mail@example.com";
        var hatData = new List<HatDTO>
        {
            new(HatType.Student, new Dictionary<string, object>()
                {
                    { "StudyField", "Computer Science" },
                    { "StudyDegree",  3 }
                }),
            new(HatType.Academic, new Dictionary<string, object>()
                {
                    {"ResearchField", "Distributed Systems" }
                })
        };

        await sut.SignUpAsync(
            accountId,
            fullName,
            contactEmail,
            hatData);

        // ACT
        var signUserUp = async () => await sut.SignUpAsync(
            accountId,
            fullName,
            contactEmail,
            hatData);

        // ASSERT
        await signUserUp.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_can_update_own_contact_email()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        var contributors = new MongoDbContributorsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var contributorId = Guid.NewGuid().ToString();
        var fullName = "John Doe";
        var contactEmail = "mail1@example.com";
        var newContactEmail = "mail2@example.com";
        var hats = new List<Hat>() { new AcademicHat("Computer Science") };

        var existingContributor = await new ContributorFactory()
            .WithAccountId(contributorId)
            .WithFullName(fullName)
            .WithEmail(contactEmail)
            .WithHats(hats)
            .SeedAsync();

        // ACT
        await sut.UpdateSelfAsync(existingContributor.Id, existingContributor.Id, fullName, newContactEmail, hats.Select(h => HatDTO.FromHat(h)).ToList());

        // ASSERT
        var retrievedContributor = await contributors.GetByAccountIdAsync(contributorId);
        retrievedContributor!.AccountId.Should().Be(contributorId);
        retrievedContributor.FullName.Should().Be(fullName);
        retrievedContributor.Hats.Should().BeEquivalentTo(hats);
        retrievedContributor.ContactEmail.Should().Be(newContactEmail);
    }

    [Fact]
    public async void A_contributor_can_update_own_full_name()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        var contributors = new MongoDbContributorsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var contributorId = Guid.NewGuid().ToString();
        var fullName = "John Doe";
        var newName = "Johnny Doe";
        var contactEmail = "mail1@example.com";
        var hats = new List<Hat>() { new AcademicHat("Computer Science") };

        var existingContributor = await new ContributorFactory()
            .WithAccountId(contributorId)
            .WithFullName(fullName)
            .WithEmail(contactEmail)
            .WithHats(hats)
            .SeedAsync();

        // ACT
        await sut.UpdateSelfAsync(existingContributor.Id, existingContributor.Id, newName, contactEmail, hats.Select(h => HatDTO.FromHat(h)).ToList());

        // ASSERT
        var retrievedContributor = await contributors.GetByAccountIdAsync(contributorId);
        retrievedContributor!.AccountId.Should().Be(contributorId);
        retrievedContributor.Hats.Should().BeEquivalentTo(hats);
        retrievedContributor.ContactEmail.Should().Be(contactEmail);
        retrievedContributor.FullName.Should().Be(newName);
    }

    [Fact]
    public async void A_contributor_can_update_own_hats()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        var contributors = new MongoDbContributorsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var contributorId = Guid.NewGuid().ToString();
        var fullName = "John Doe";
        var contactEmail = "mail1@example.com";
        var hats = new List<Hat>() { new AcademicHat("Computer Science") };
        var newHats = new List<Hat>() { new StudentHat("Computer Science", AcademicDegree.Doctorate) };

        var existingContributor = await new ContributorFactory()
            .WithAccountId(contributorId)
            .WithFullName(fullName)
            .WithEmail(contactEmail)
            .WithHats(hats)
            .SeedAsync();

        // ACT
        await sut.UpdateSelfAsync(existingContributor.Id, existingContributor.Id, fullName, contactEmail, newHats.Select(h => HatDTO.FromHat(h)).ToList());

        // ASSERT
        var retrievedContributor = await contributors.GetByAccountIdAsync(contributorId);
        retrievedContributor!.AccountId.Should().Be(contributorId);
        retrievedContributor.FullName.Should().Be(fullName);
        retrievedContributor.ContactEmail.Should().Be(contactEmail);
        retrievedContributor.Hats.Should().BeEquivalentTo(newHats);
    }

    [Fact]
    public async void A_contributor_cannot_update_another()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        var contributors = new MongoDbContributorsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var requester = await new ContributorFactory().SeedAsync();
        var contributor = await new ContributorFactory().SeedAsync();

        // ACT
        var updatingAnotherContributor = async () => await sut.UpdateSelfAsync(
            requester.Id,
            contributor.Id,
            contributor.FullName + "*",
            contributor.ContactEmail,
            contributor.Hats.Select(h => HatDTO.FromHat(h)).ToList());

        // ASSERT
        await updatingAnotherContributor.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_can_remove_themselves_from_the_platform()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        accountManagement.Setup(m => m.RemoveAccountAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        var contributors = new MongoDbContributorsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            domainEvents,
            new NullLogger<ContributorsService>());

        var contributor = await new ContributorFactory()
            .SeedAsync();

        // ACT
        await sut.RemoveSelfAsync(contributor.Id, contributor.Id);

        // ASSERT
        var retrievedContributor = await contributors.GetByIdAsync(contributor.Id);
        retrievedContributor.Should().BeNull();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
        retrievedDomainEvents[0].Should().BeOfType<ContributorRemoved>();
    }

    [Fact]
    public async void A_contributor_cannot_remove_others_from_the_platform()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().
            AddUserSecrets(GetType().Assembly)
            .Build();

        var accountManagement = new Mock<IAccountManagementService>(MockBehavior.Strict);
        var contributors = new MongoDbContributorsRepository(config);

        var sut = new ContributorsService(
            contributors,
            accountManagement.Object,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ContributorsService>());

        var requester = await new ContributorFactory().SeedAsync();
        var contributor = await new ContributorFactory().SeedAsync();

        // ACT
        var removingAnotherContributor = async () => await sut.RemoveSelfAsync(requester.Id, contributor.Id);

        // ASSERT
        await removingAnotherContributor.Should().ThrowAsync<InvalidOperationException>();
        var retrievedContributor = await contributors.GetByIdAsync(contributor.Id);
        retrievedContributor.Should().NotBeNull();
    }
}
