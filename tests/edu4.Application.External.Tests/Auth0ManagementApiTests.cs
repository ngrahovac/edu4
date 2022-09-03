using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using edu4.Domain.Users;
using edu4.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace edu4.Application.External.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("External services integration tests")]

public class Auth0ManagementApiTests
{
    private record Auth0UserRole(string Id, string Name, string Description);


    /// <summary>
    /// Not automated: the test assumes an existing user under Auth0 tenant
    /// and no user document with the same account id.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task User_account_is_assigned_contributor_role_after_completing_signup()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var userId = "google-oauth2|100703511693509013031";

        // setting up the client
        var tokenFetchingService = new Auth0ManagementApiAccessTokenFetchingService(config);
        var token = await tokenFetchingService.FetchAsync();

        var auth0ManagementApiHttpClient = new HttpClient()
        {
            BaseAddress = new Uri($"https://{config["Auth0:Domain"]}/api/v2/")
        };

        auth0ManagementApiHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        await new DbUtils(config).CleanDatabaseAsync();

        var users = new MongoDbUsersRepository(config);
        var accountManagement = new Auth0AccountManagementService(auth0ManagementApiHttpClient);

        var sut = new UsersService(users, accountManagement);

        // ACT
        await sut.SignUpAsync(userId, "mail@example.com", new List<Hat>() { new StudentHat("Computer Science") });

        // ASSERT
        await Task.Delay(31);

        var fetchAccountRolesResponse = await auth0ManagementApiHttpClient.GetAsync($"users/{userId}/roles");
        var content = await fetchAccountRolesResponse.Content.ReadFromJsonAsync<List<Auth0UserRole>>();

        content!.Select(r => r.Name).Should().Contain("Contributor");
    }
}
