using System.Reflection;
using Peer.Domain.Contributors;

namespace Peer.Tests.Utils.Factories;
public class ContributorsFactory
{
    private string _accountId = Guid.NewGuid().ToString();
    private string _fullName = "John Doe";
    private string _email = "mail@example.com";
    private List<Hat> _hats = new();
    private bool _removed = false;

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

    public ContributorsFactory WithRemoved(bool removed)
    {
        _removed = removed;
        return this;
    }

    public Contributor Build()
    {
        var contributor = new Contributor(
            _accountId,
            _fullName,
            _email,
            _hats);

        if (_removed)
        {
            MakeRemovedViaReflection(contributor);
        }

        return contributor;
    }

    private void MakeRemovedViaReflection(Contributor contributor)
    {
        var removedProp = typeof(Contributor).GetProperty(
            nameof(contributor.Removed),
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) ??
            throw new InvalidOperationException("Error making the project removed via reflection");

        removedProp.SetValue(contributor, true);
    }
}
