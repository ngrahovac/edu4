using System.Net.Http.Json;
using edu4.Application.External;

namespace edu4.Infrastructure;
public class Auth0AccountManagementService : IAccountManagementService
{
    private readonly HttpClient _auth0ManagementApiClient;

    private record RoleAssignmentInputModel(string[] Roles);

    public Auth0AccountManagementService(HttpClient auth0ManagementApiClient) =>
        _auth0ManagementApiClient = auth0ManagementApiClient;

    public async Task MarkUserSignedUpAsync(string accountId)
    {
        var roles = new[] { "rol_x9uzdhEDWHNRfDD2" };

        var response = await _auth0ManagementApiClient.PostAsJsonAsync(
            $"users/{accountId}/roles",
            new RoleAssignmentInputModel(roles));

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Exception while making Auth0 Management API call: could not mark user with id {accountId} signed up.");
        }
    }
}
