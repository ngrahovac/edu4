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

namespace Peer.Tests.Application;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class UsersServiceTests
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

        var sut = new ContributorsService(users, accountManagementMock.Object, new NullLogger<ContributorsService>());

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
}
