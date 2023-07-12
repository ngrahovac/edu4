using Peer.Domain.Contributors;

namespace Peer.Tests.Utils.Factories;
public class ContributorsFactory
{
    private string _accountId = Guid.NewGuid().ToString();
    private string _fullName = "John Doe";
    private string _email = "mail@example.com";
    private List<Hat> _hats = new();

    public ContributorsFactory WithAccountId(string accountId)
    {
        _accountId = accountId;
        return this;
    }

    public ContributorsFactory WithFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public ContributorsFactory WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ContributorsFactory WithHats(List<Hat> hats)
    {
        _hats = hats;
        return this;
    }

    public Contributor Build()
    {
        return new Contributor(
            _accountId,
            _fullName,
            _email,
            _hats);
    }
}
