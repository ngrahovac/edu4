namespace Peer.API.Utils;

public interface IAccountIdExtractionService
{
    public string ExtractAccountIdFromHttpRequest(HttpRequest request);
}
