using edu4.Application.External;
using edu4.Domain.Users;
using Microsoft.Extensions.Logging;

namespace edu4.Application;

public class UsersService
{
    private readonly IUsersRepository _users;
    private readonly IAccountManagementService _accountManagement;
    private readonly ILogger<UsersService> _logger;

    public UsersService(
        IUsersRepository users,
        IAccountManagementService accountManagement,
        ILogger<UsersService> logger)
    {
        _users = users;
        _accountManagement = accountManagement;
        _logger = logger;
    }

    /// <summary>
    /// Assumes a valid account id registered under the tenant.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="contactEmail"></param>
    /// <param name="hats"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<User> SignUpAsync(
        string accountId,
        string fullName,
        string contactEmail,
        List<Hat> hats)
    {
        var user = new User(
            accountId,
            fullName,
            contactEmail,
            hats);

        if (await _users.GetByAccountIdAsync(accountId) is not null)
        {
            _logger.LogError("User with {AccountId} already signed in", accountId);
            throw new InvalidOperationException($"User with account id {accountId} already signed up");
        }

        await _users.AddAsync(user);
        await _accountManagement.MarkUserSignedUpAsync(accountId);  // Q: cross-process transaction?

        _logger.LogInformation("User {User} successfully signed in", user);

        return user;
    }
}
