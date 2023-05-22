using Peer.Domain.Contributors;
using Peer.Infrastructure;
using Microsoft.Extensions.Configuration;
using Peer.Application.Contracts;

namespace Peer.Tests.Application.TestData;
internal class ContributorFactory
{
    private string _accountId = Guid.NewGuid().ToString();
    private string _fullName = "John Doe";
    private string _email = "mail@example.com";
    private List<Hat> _hats = new();
    private readonly IContributorsRepository _users;

    public ContributorFactory()
    {
        var config = new ConfigurationBuilder().AddUserSecrets(typeof(ContributorFactory).Assembly)
            .Build();

        _users = new MongoDbContributorsRepository(config);
    }
    public ContributorFactory WithAccountId(string accountId)
    {
        _accountId = accountId;
        return this;
    }

    public ContributorFactory WithFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public ContributorFactory WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ContributorFactory WithHats(List<Hat> hats)
    {
        _hats = hats;
        return this;
    }

    public async Task<Contributor> SeedAsync()
    {
        var user = new Contributor(
            _accountId,
            _fullName,
            _email,
            _hats);

        await _users.AddAsync(user);

        return user;
    }
}
