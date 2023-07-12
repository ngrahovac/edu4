using Microsoft.Extensions.Configuration;
using Peer.Application.Contracts;
using Peer.Domain.Contributors;
using Peer.Infrastructure;
using Peer.Tests.Utils.Factories;

namespace Peer.Test.Utils.Seeders;

public class ContributorsSeeder
{
    private readonly IContributorsRepository _contributors;
    private readonly ContributorsFactory _contributorsFactory;

    public ContributorsSeeder(IConfiguration configuration)
    {
        _contributors = new MongoDbContributorsRepository(configuration);
        _contributorsFactory = new ContributorsFactory();
    }

    public ContributorsSeeder WithAccountId(string accountId)
    {
        _contributorsFactory.WithAccountId(accountId);
        return this;
    }

    public ContributorsSeeder WithFullName(string fullName)
    {
        _contributorsFactory.WithFullName(fullName);
        return this;
    }

    public ContributorsSeeder WithEmail(string email)
    {
        _contributorsFactory.WithEmail(email);
        return this;
    }

    public ContributorsSeeder WithHats(List<Hat> hats)
    {
        _contributorsFactory.WithHats(hats);
        return this;
    }

    public async Task<Contributor> SeedAsync()
    {
        var contributor = _contributorsFactory.Build();

        await _contributors.AddAsync(contributor);

        return contributor;
    }
}
