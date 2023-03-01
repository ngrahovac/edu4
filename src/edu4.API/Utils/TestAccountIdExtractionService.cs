namespace edu4.API.Utils;

public class TestAccountIdExtractionService : IAccountIdExtractionService
{
    private readonly IConfiguration _configuration;

    public TestAccountIdExtractionService(IConfiguration configuration)
        => _configuration = configuration;

    public string ExtractAccountIdFromHttpRequest(HttpRequest request)
        => _configuration["TestUserId"];
}
