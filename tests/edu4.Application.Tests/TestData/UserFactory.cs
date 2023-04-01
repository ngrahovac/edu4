using edu4.Application.Contracts;
using edu4.Domain.Users;
using edu4.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace edu4.Application.Tests.TestData;
internal class UserFactory
{
    private string _accountId = Guid.NewGuid().ToString();
    private string _fullName = "John Doe";
    private string _email = "mail@example.com";
    private List<Hat> _hats = new();
    private readonly IUsersRepository _users;

    public UserFactory()
    {
        var config = new ConfigurationBuilder().AddUserSecrets(typeof(UserFactory).Assembly)
            .Build();

        _users = new MongoDbUsersRepository(config);
    }
    public UserFactory WithAccountId(string accountId)
    {
        _accountId = accountId;
        return this;
    }

    public UserFactory WithFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public UserFactory WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserFactory WithHats(List<Hat> hats)
    {
        _hats = hats;
        return this;
    }

    public async Task<User> SeedAsync()
    {
        var user = new User(
            _accountId,
            _fullName,
            _email,
            _hats);

        await _users.AddAsync(user);

        return user;
    }
}