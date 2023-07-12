using Peer.Domain.Common;

namespace Peer.Domain.Contributors;
public class Contributor : AbstractAggregateRoot
{
    public string AccountId { get; }

    public string FullName { get; private set; }

    public string ContactEmail { get; private set; }

    private readonly HashSet<Hat> _hats = new();

    public IReadOnlyList<Hat> Hats => _hats.ToList();

    public bool Removed { get; private set; }


    public Contributor(
        string accountId,
        string fullName,
        string contactEmail,
        List<Hat> hats)
    {
        AccountId = accountId;
        FullName = fullName;
        ContactEmail = contactEmail;

        foreach (var hat in hats)
        {
            if (_hats.Contains(hat))
            {
                throw new InvalidOperationException("A user cannot have duplicate hats");
            }

            _hats.Add(hat);
        }

        Removed = false;
    }


    public void UpdateContactEmail(string contactEmail)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot update email of a removed contributor");
        }

        ContactEmail = contactEmail;
    }

    public void UpdateFullName(string newFullName)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot update name of a removed contributor");
        }

        FullName = newFullName;
    }

    public void UpdateHats(IReadOnlyCollection<Hat> updatedHats)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot update hats of a removed contributor");
        }

        _hats.Clear();

        foreach (var h in updatedHats)
        {
            _hats.Add(h);
        }
    }

    public void Remove()
    {
        if (Removed)
        {
            throw new InvalidOperationException("The contributor has aleady been removed");
        }

        Removed = true;
        RaiseDomainEvent(new ContributorRemoved(this));
    }
}
