using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Peer.Infrastructure;

public class Auth0ManagementApiAccessTokenFetchingService
{
    private readonly IConfiguration _config;
    private record Auth0ManagementApiFetchTokenPayload(
        string Grant_type,
        string Client_id,
        string Client_secret,
        string Audience);

    private record Auth0ManagementApiFetchTokenResponse(
        string Access_token,
        int Expires_in,
        string Scope,
        string Token_type);

    public Auth0ManagementApiAccessTokenFetchingService(IConfiguration config) =>
        _config = config;

    public async Task<string> FetchAsync()
    {
        var client = new HttpClient()
        {
            BaseAddress = new Uri($"https://{_config["Auth0:Domain"]}/")
        };

        var response = await client.PostAsJsonAsync(
            "oauth/token",
            new Auth0ManagementApiFetchTokenPayload(
                "client_credentials",
                _config["Auth0:edu4.API:ClientID"],
                _config["Auth0:edu4.API:ClientSecret"],
                _config["Auth0:ManagementApiAudience"]
                )
            );

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Exception while fetching an Auth0 Management API access token");
        }

        var deserializedContent = await response.Content.ReadFromJsonAsync<Auth0ManagementApiFetchTokenResponse>() ??
            throw new InvalidOperationException("Exception while deserializing response payload after fetching an Auth0 Management API access token");

        return deserializedContent.Access_token;
    }
}
