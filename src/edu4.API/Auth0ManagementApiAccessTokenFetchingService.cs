namespace edu4.API;

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

        var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token");
        var payload = new Auth0ManagementApiFetchTokenPayload(
            "client_credentials",
            _config["Auth0:edu4.API:ClientID"],
            _config["Auth0:edu4.API:ClientSecret"],
            _config["Auth0:ManagementApiAudience"]);
        request.Content = JsonContent.Create(payload, typeof(Auth0ManagementApiFetchTokenPayload));

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Exception while fetching an Auth0 Management API access token");
        }

        var deserializedContent = await response.Content.ReadFromJsonAsync<Auth0ManagementApiFetchTokenResponse>();

        if (deserializedContent is null)
        {
            throw new InvalidOperationException(
                "Exception while deserializing response payload after fetching an Auth0 Management API access token");
        }

        return deserializedContent.Access_token;
    }
}
