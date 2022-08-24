using edu4.Domain.Users;

namespace edu4.Application;

public class UsersService
{
    private readonly IUsersRepository _users;

    public UsersService(IUsersRepository users) =>
        _users = users;

    public async Task<User> SignUpAsync(
        string accountId,
        string contactEmail,
        List<Hat> hats)
    {
        var user = new User(accountId, contactEmail, hats);

        if (await _users.GetByAccountIdAsync(accountId) is not null)
        {
            throw new InvalidOperationException($"User with account id {accountId} already signed up");
        }

        await _users.AddAsync(user);

        return user;
    }
}
