using edu4.Domain.Users;
using edu4.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace edu4.Application.Tests;
internal static class TestDataFactory
{
    public static async Task<User> CreateUserAsync(
        string accountId,
        string fullName,
        string contactEmail,
        List<Hat> hats)
    {
        var config = new ConfigurationBuilder().AddUserSecrets(typeof(TestDataFactory).Assembly).Build();
        var users = new MongoDbUsersRepository(config);

        var user = new User(
            accountId,
            fullName,
            contactEmail,
            hats);

        await users.AddAsync(user);

        return user;
    }
}
