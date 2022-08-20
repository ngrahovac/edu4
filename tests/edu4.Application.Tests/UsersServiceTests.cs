using System.Diagnostics.CodeAnalysis;
using edu4.Domain.Users;
using edu4.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace edu4.Application.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("Integration tests")]
public class UsersServiceTests
{
    [Fact]
    public async void Successfully_signs_up_a_new_user_providing_valid_email_account_id_and_hat_list()
    {
        // arrange
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(MongoDbUsersRepository).Assembly)
            .Build();

        await new DbTestUtils(config).CleanDatabaseAsync();

        var users = new MongoDbUsersRepository(config);

        var sut = new UsersService(users);

        var accountId = "google-oauth2|0";
        var contactEmail = "mail@example.com";
        var hats = new List<Hat>
        {
            new StudentHat("Computer Science", AcademicDegree.Doctorate),
            new AcademicHat("Distributed Systems")
        };

        // act
        var signedUpUser = await sut.SignUp(accountId, contactEmail, hats);

        // assert user got assigned id by db driver
        signedUpUser.Id.Should().NotBe(Guid.Empty);

        // assert db state through repository
        var retrievedUser = await users.GetByIdAsync(signedUpUser.Id);

        retrievedUser!.Id.Should().Be(signedUpUser.Id);
        retrievedUser.AccountId.Should().Be(accountId);
        retrievedUser.ContactEmail.Should().Be(contactEmail);
        retrievedUser.Hats.Should().BeEquivalentTo(hats);
    }

    [Fact]
    public async void Throws_if_signing_up_a_user_with_account_id_belonging_to_an_existing_user()
    {
        // arrange
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(MongoDbUsersRepository).Assembly)
            .Build();

        await new DbTestUtils(config).CleanDatabaseAsync();

        var users = new MongoDbUsersRepository(config);

        var sut = new UsersService(users);

        var accountId = "google-oauth2|0";
        var contactEmail = "mail@example.com";
        var hats = new List<Hat>
        {
            new StudentHat("Computer Science", AcademicDegree.Doctorate),
            new AcademicHat("Distributed Systems")
        };

        await sut.SignUp(accountId, contactEmail, hats);

        // act
        var signUserUp = async () => await sut.SignUp(accountId, contactEmail, hats);

        // assert
        await signUserUp.Should().ThrowAsync<InvalidOperationException>();
    }
}
