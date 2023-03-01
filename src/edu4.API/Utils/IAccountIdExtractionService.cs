namespace edu4.API.Utils;

public interface IAccountIdExtractionService
{
    public string ExtractAccountIdFromHttpRequest(HttpRequest request);
}
