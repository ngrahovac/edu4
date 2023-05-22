using Peer.Domain.Common;

namespace Peer.Domain.Contributors;
public class Contributor : AbstractAggregateRoot
{
    public string AccountId { get; }

    public string FullName { get; private set; }

    public string ContactEmail { get; private set; }

    private readonly HashSet<Hat> _hats = new();

    public IReadOnlyList<Hat> Hats => _hats.ToList();


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
    }


    public void UpdateContactEmail(string contactEmail) =>
        ContactEmail = contactEmail;

    public void UpdateFullName(string newFullName) =>
        FullName = newFullName;

    public void UpdateHats(IReadOnlyCollection<Hat> updatedHats)
    {
        _hats.Clear();

        foreach (var h in updatedHats)
        {
            _hats.Add(h);
        }
    }
}
