using edu4.Domain.Common;

namespace edu4.Domain.Contributors;
public class User : AbstractAggregateRoot
{
    public string AccountId { get; }

    public string FullName { get; }

    public string ContactEmail { get; }

    private readonly HashSet<Hat> _hats = new();

    public IReadOnlyList<Hat> Hats => _hats.ToList();


    public User(
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
}
