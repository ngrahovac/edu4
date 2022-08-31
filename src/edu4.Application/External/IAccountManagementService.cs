namespace edu4.Application.External;
public interface IAccountManagementService
{
    public Task MarkUserSignedUpAsync(string accountId);
}
