namespace Peer.Application.Contracts;
public interface IAccountManagementService
{
    public Task MarkUserSignedUpAsync(string accountId);
}
