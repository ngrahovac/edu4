namespace edu4.Application.Contracts;
public interface IAccountManagementService
{
    public Task MarkUserSignedUpAsync(string accountId);
}
